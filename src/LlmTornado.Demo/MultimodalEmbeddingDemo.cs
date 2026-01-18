using LlmTornado.Embedding;
using LlmTornado.Embedding.Models;

namespace LlmTornado.Demo;

public class MultimodalEmbeddingDemo : DemoBase
{
    private static string? GetImagePath(string imageName)
    {
        string? dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;

        if (dir is null)
        {
            return null;
        }
        
        return Path.Combine(dir, "LlmTornado.Demo", "Static", "Images", imageName);
    }
    
    private static string? GetImageAsBase64(string imageName)
    {
        string? path = GetImagePath(imageName);

        if (path is null)
        {
            return null;
        }

        byte[] imageArray = File.ReadAllBytes(path);
        return $"data:image/jpeg;base64,{Convert.ToBase64String(imageArray)}";
    }
    
    [TornadoTest]
    public static async Task EmbedImageBase64()
    {
        string? base64 = GetImageAsBase64("catBoi.jpg");
        Assert.That(base64, Is.NotNull);

        MultimodalEmbeddingRequest request = new MultimodalEmbeddingRequest(MultimodalEmbeddingModel.Voyage.Gen35.Multimodal, [
            new MultimodalInput([
                new MultimodalContentText("a cat"),
                new MultimodalContentImageBase64(base64!)
            ])
        ]);
        
        MultimodalEmbeddingResult? result = await Program.ConnectMulti().MultimodalEmbeddings.CreateMultimodalEmbedding(request);
        
        Assert.That(result, Is.NotNull);
        Assert.That(result.Data, Is.NotNull);
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data[0].Embedding, Is.InstanceOf<MultimodalEmbeddingValueFloat>());

        if (result.Data[0].Embedding is MultimodalEmbeddingValueFloat floatVec)
        {
            Console.WriteLine($"Embedding received, dimensions: {floatVec.Values.Length}");
        }
    }
    
    private static string? GetVideoPath(string videoName)
    {
        string? dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.Parent?.FullName;

        if (dir is null)
        {
            return null;
        }
        
        return Path.Combine(dir, "LlmTornado.Demo", "Static", "Files", videoName);
    }
    
    private static string? GetVideoAsBase64(string videoName)
    {
        string? path = GetVideoPath(videoName);

        if (path is null)
        {
            return null;
        }

        byte[] videoArray = File.ReadAllBytes(path);
        return $"data:video/mp4;base64,{Convert.ToBase64String(videoArray)}";
    }

    [TornadoTest]
    public static async Task EmbedVideoBase64()
    {
        string? base64 = GetVideoAsBase64("video.mp4");
        Assert.That(base64, Is.NotNull);

        MultimodalEmbeddingRequest request = new MultimodalEmbeddingRequest(MultimodalEmbeddingModel.Voyage.Gen35.Multimodal, [
            new MultimodalInput([
                new MultimodalContentText("a short video clip"),
                new MultimodalContentVideoBase64(base64!)
            ])
        ]);
        
        MultimodalEmbeddingResult? result = await Program.ConnectMulti().MultimodalEmbeddings.CreateMultimodalEmbedding(request);
        
        Assert.That(result, Is.NotNull);
        Assert.That(result.Data, Is.NotNull);
        Assert.That(result.Data.Count, Is.EqualTo(1));
        Assert.That(result.Data[0].Embedding, Is.InstanceOf<MultimodalEmbeddingValueFloat>());

        if (result.Data[0].Embedding is MultimodalEmbeddingValueFloat floatVec)
        {
            Console.WriteLine($"Video embedding received, dimensions: {floatVec.Values.Length}");
        }
    }
}
