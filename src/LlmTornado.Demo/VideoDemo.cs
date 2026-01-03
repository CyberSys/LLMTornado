using LlmTornado.Common;
using LlmTornado.Videos;
using LlmTornado.Videos.Models;

namespace LlmTornado.Demo;

public class VideoDemo : DemoBase
{
    [TornadoTest, Flaky("expensive")]
    public static async Task GenerateSimpleVideoGoogle()
    {
        TornadoApi api = Program.Connect();
        
        byte[] bytes = await File.ReadAllBytesAsync("Static/Images/bull.jpeg");
        string base64 = $"{Convert.ToBase64String(bytes)}";

        
        VideoGenerationRequest request = new VideoGenerationRequest(
            "A bull moves confidently forward, smiling and waving.",
            VideoModel.Google.Veo.V31Fast,
            duration: VideoDuration.Seconds8,
            aspectRatio: VideoAspectRatio.Widescreen,
            resolution: VideoResolution.HD
        )
        {
            Image = new VideoImage(base64, "image/jpeg")
        };
        
        const string outputPath = "output/generated_video.mp4";
        
        Console.WriteLine("Starting video generation...");
        HttpCallResult<VideoJob>? result = await api.Videos.CreateAndWait(request, new VideoJobEvents
        {
            OnPoll = async (result, index, elapsed) =>
            {
                Console.WriteLine(result.Progress is not null ? $"[Poll #{index}] Progress: {result.Progress}% - Elapsed: {elapsed.TotalSeconds:F1}s" : $"[Poll #{index}] Status: {(result?.Done == true ? "Done" : "In Progress")} - Elapsed: {elapsed.TotalSeconds:F1}s");
                await ValueTask.CompletedTask;
            },
            OnFinished = async (result, videoStream) =>
            {
                Console.WriteLine($"Video generation completed!");
                string savedTo = await videoStream.SaveToFileAsync(outputPath);
                Console.WriteLine($"Video saved to: {savedTo}");
            }
        });

        Console.WriteLine(result.Data?.Done == true ? $"Process completed. Check {outputPath} for the video." : "Video generation failed or returned no results.");
    }
    
    [TornadoTest, Flaky("expensive")]
    public static async Task GenerateSimpleVideoOpenAi()
    {
        TornadoApi api = Program.Connect();
        
        byte[] bytes = await File.ReadAllBytesAsync("Static/Images/bull_hd.jpeg");
        string base64 = $"{Convert.ToBase64String(bytes)}";

        
        VideoGenerationRequest request = new VideoGenerationRequest(
            "A bull moves confidently forward, smiling and waving.",
            VideoModel.OpenAi.Sora.Sora2,
            duration: VideoDuration.Seconds4,
            aspectRatio: VideoAspectRatio.Widescreen,
            resolution: VideoResolution.HD
        )
        {
            Image = new VideoImage(base64, "image/jpeg")
        };
        
        const string outputPath = "output/generated_video.mp4";
        
        Console.WriteLine("Starting video generation...");
        HttpCallResult<VideoJob>? result = await api.Videos.CreateAndWait(request, new VideoJobEvents
        {
            OnPoll = async (result, index, elapsed) =>
            {
                Console.WriteLine(result.Progress is not null ? $"[Poll #{index}] Progress: {result.Progress}% - Elapsed: {elapsed.TotalSeconds:F1}s" : $"[Poll #{index}] Status: {(result?.Done == true ? "Done" : "In Progress")} - Elapsed: {elapsed.TotalSeconds:F1}s");
                await ValueTask.CompletedTask;
            },
            OnFinished = async (result, videoStream) =>
            {
                Console.WriteLine($"Video generation completed!");
                string savedTo = await videoStream.SaveToFileAsync(outputPath);
                Console.WriteLine($"Video saved to: {savedTo}");
            }
        });

        Console.WriteLine(result.Data?.Done == true ? $"Process completed. Check {outputPath} for the video." : "Video generation failed or returned no results.");
    }
}