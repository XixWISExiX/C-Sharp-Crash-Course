using System.Diagnostics;
using System.Net.Sockets;

namespace ContactManager.Tests.Infrastructure;

public sealed class DotnetProcess : IAsyncDisposable
{
    private readonly Process _process;
    private readonly Func<string> _getLogs;

    public DotnetProcess(Process process, Func<string> getLogs)
    {
        _process = process;
        _getLogs = getLogs;
    }

    public bool HasExited => _process.HasExited;

    public int? ExitCode => _process.HasExited ? _process.ExitCode : null;

    public string GetRecentLogs() => _getLogs();

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (!_process.HasExited)
                _process.Kill(entireProcessTree: true);
        }
        catch { /* ignore */ }

        try { _process.Dispose(); } catch { /* ignore */ }

        await Task.CompletedTask;
    }
}


public static class ProcessRunner
{
    public static int GetFreePort()
    {
        var listener = new TcpListener(System.Net.IPAddress.Loopback, 0);
        listener.Start();
        var port = ((System.Net.IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public static DotnetProcess StartGrpcServer(string repoRoot, int port)
    {
        var args =
            $"run --project src/ContactManager.Grpc/ContactManager.Grpc.csproj --urls http://localhost:{port}";

        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = repoRoot,          // MUST be solution root
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var p = new Process { StartInfo = psi, EnableRaisingEvents = true };

        var stdout = new List<string>();
        var stderr = new List<string>();

        p.OutputDataReceived += (_, e) => { if (e.Data is not null) stdout.Add(e.Data); };
        p.ErrorDataReceived  += (_, e) => { if (e.Data is not null) stderr.Add(e.Data); };

        if (!p.Start())
            throw new InvalidOperationException("Failed to start gRPC process.");

        p.BeginOutputReadLine();
        p.BeginErrorReadLine();

        return new DotnetProcess(p, () =>
        {
            // snapshot logs when we need them
            var outText = string.Join("\n", stdout.TakeLast(80));
            var errText = string.Join("\n", stderr.TakeLast(80));
            return $"--- gRPC stdout (last 80) ---\n{outText}\n\n--- gRPC stderr (last 80) ---\n{errText}\n";
        });
    }


    public static async Task WaitForPortAsync(string host, int port, TimeSpan timeout, DotnetProcess proc)
    {
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (proc.HasExited)
            {
                throw new InvalidOperationException(
                    $"gRPC process exited early with code {proc.ExitCode}.\n{proc.GetRecentLogs()}");
            }

            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(150));
                if (completed == connectTask && client.Connected) return;
            }
            catch
            {
                // keep trying
            }

            await Task.Delay(150);
        }

        throw new TimeoutException($"Port {port} on {host} did not open within {timeout.TotalSeconds:0.##}s.\n{proc.GetRecentLogs()}");
    }

}
