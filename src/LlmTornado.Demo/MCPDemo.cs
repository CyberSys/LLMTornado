using LlmTornado.Mcp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace LlmTornado.Demo;

public class MCPDemo
{
    [TornadoTest("TestMCPFileSystem")]
    public static async Task RunAllTests()
    {
        Console.WriteLine("=== MCP Filesystem Diagnostic Tests ===\n");

        // Test 1: Raw process test (bypasses LlmTornado entirely)
        await Test1_RawProcessAsync();

        // Test 2: Basic LlmTornado initialization
        await Test2_BasicInitAsync();

        // Test 3: List tools
        await Test3_ListToolsAsync();

        // Test 4: Call list_allowed_directories
        await Test4_ListAllowedDirectoriesAsync();

        // Test 5: Call list_directory with a path
        await Test5_ListDirectoryAsync();

        Console.WriteLine("\n=== All Tests Complete ===");
    }

    /// <summary>
    /// Test 1: Bypass LlmTornado - test raw MCP server process
    /// </summary>
    private static async Task Test1_RawProcessAsync()
    {
        Console.WriteLine("--- Test 1: Raw Process Test ---");
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "npx",
                Arguments = "-y @modelcontextprotocol/server-filesystem C:\\Users\\johnl\\source\\repos\\LombdaStudio",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            if (process == null)
            {
                Console.WriteLine("FAIL: Could not start process");
                return;
            }

            // Send initialize request
            var initRequest = new
            {
                jsonrpc = "2.0",
                id = 1,
                method = "initialize",
                @params = new
                {
                    protocolVersion = "2024-11-05",
                    capabilities = new { },
                    clientInfo = new { name = "test", version = "1.0" }
                }
            };

            string json = JsonSerializer.Serialize(initRequest);
            await process.StandardInput.WriteLineAsync(json);
            await process.StandardInput.FlushAsync();

            // Read response with timeout
            var readTask = process.StandardOutput.ReadLineAsync();
            if (await Task.WhenAny(readTask, Task.Delay(5000)) == readTask)
            {
                var response = await readTask;
                Console.WriteLine($"PASS: Got response: {response?.Substring(0, Math.Min(100, response?.Length ?? 0))}...");
            }
            else
            {
                Console.WriteLine("FAIL: Timeout waiting for response");
            }

            process.Kill();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Test 2: Basic LlmTornado MCP initialization
    /// </summary>
    private static async Task Test2_BasicInitAsync()
    {
        Console.WriteLine("--- Test 2: LlmTornado Basic Init ---");
        MCPServer? server = null;
        try
        {
            server = new MCPServer(
                serverLabel: "filesystem-test",
                command: "npx",
                arguments: ["-y", "@modelcontextprotocol/server-filesystem", "C:\\Users\\johnl\\source\\repos\\LombdaStudio"],
                workingDirectory: "",
                environmentVariables: new Dictionary<string, string>(),
                allowedTools: Array.Empty<string>()
            );

            await server.InitializeAsync();

            if (server.McpClient != null)
            {
                Console.WriteLine("PASS: McpClient initialized");
            }
            else
            {
                Console.WriteLine("FAIL: McpClient is null after init");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
        finally
        {
            if (server is IDisposable d) d.Dispose();
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Test 3: List available tools
    /// </summary>
    private static async Task Test3_ListToolsAsync()
    {
        Console.WriteLine("--- Test 3: List Tools ---");
        MCPServer? server = null;
        try
        {
            server = new MCPServer(
                serverLabel: "filesystem-test",
                command: "npx",
                arguments: ["-y", "@modelcontextprotocol/server-filesystem", "C:\\Users\\johnl\\source\\repos\\LombdaStudio"],
                workingDirectory: "",
                environmentVariables: new Dictionary<string, string>(),
                allowedTools: Array.Empty<string>()
            );

            await server.InitializeAsync();

            var tools = await server.McpClient!.ListTornadoToolsAsync();
            Console.WriteLine($"PASS: Found {tools.Count} tools:");
            foreach (var tool in tools)
            {
                var name = tool.Function?.Name ?? tool.Custom?.Name ?? "unknown";
                Console.WriteLine($"  - {name}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
        }
        finally
        {
            if (server is IDisposable d) d.Dispose();
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Test 4: Call list_allowed_directories (the failing tool)
    /// </summary>
    private static async Task Test4_ListAllowedDirectoriesAsync()
    {
        Console.WriteLine("--- Test 4: Call list_allowed_directories ---");
        MCPServer? server = null;
        try
        {
            server = new MCPServer(
                serverLabel: "filesystem-test",
                command: "npx",
                arguments: ["-y", "@modelcontextprotocol/server-filesystem", "C:\\Users\\johnl\\source\\repos\\LombdaStudio"],
                workingDirectory: "",
                environmentVariables: new Dictionary<string, string>(),
                allowedTools: ["list_allowed_directories"]
            );

            await server.InitializeAsync();

            // Try calling the tool directly via MCP client
            var result = await server.McpClient!.CallToolAsync("list_allowed_directories", new Dictionary<string, object>());

            Console.WriteLine($"PASS: Tool returned: {JsonSerializer.Serialize(result)}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
        finally
        {
            if (server is IDisposable d) d.Dispose();
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Test 5: Call list_directory with a path
    /// </summary>
    private static async Task Test5_ListDirectoryAsync()
    {
        Console.WriteLine("--- Test 5: Call list_directory ---");
        MCPServer? server = null;
        try
        {
            server = new MCPServer(
                serverLabel: "filesystem-test",
                command: "npx",
                arguments: ["-y", "@modelcontextprotocol/server-filesystem", "C:\\Users\\johnl\\source\\repos\\LombdaStudio"],
                workingDirectory: "",
                environmentVariables: new Dictionary<string, string>(),
                allowedTools: ["list_directory"]
            );

            await server.InitializeAsync();

            var args = new Dictionary<string, object>
            {
                ["path"] = "C:\\Users\\johnl\\source\\repos\\LombdaStudio"
            };

            var result = await server.McpClient!.CallToolAsync("list_directory", args);

            Console.WriteLine($"PASS: Tool returned: {JsonSerializer.Serialize(result).Substring(0, Math.Min(200, JsonSerializer.Serialize(result).Length))}...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"FAIL: {ex.GetType().Name}: {ex.Message}");
            Console.WriteLine($"Stack: {ex.StackTrace}");
        }
        finally
        {
            if (server is IDisposable d) d.Dispose();
        }
        Console.WriteLine();
    }
}
