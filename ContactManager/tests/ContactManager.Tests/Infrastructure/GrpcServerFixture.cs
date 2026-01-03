using Xunit;

namespace ContactManager.Tests.Infrastructure;

public sealed class GrpcServerFixture : IAsyncLifetime
{
    public DotnetProcess? Process { get; private set; }
    public int Port { get; private set; }
    public string Url => $"http://localhost:{Port}";

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null)
        {
            // pick ONE of these markers that definitely exists at your repo root:
            // - the .sln file
            // - the src folder
            // - a known file like global.json
            if (dir.GetFiles("*.sln").Any() || dir.GetDirectories("src").Any())
                return dir.FullName;

            dir = dir.Parent;
        }

        throw new InvalidOperationException(
            "Could not locate repo root. Expected to find a *.sln file or a 'src' directory above the test output folder.");
    }


    public async Task InitializeAsync()
    {
        // dotnet test runs with working dir usually = repo root (where the .sln is).
        // If that's not true for you, set repoRoot to the solution folder explicitly.
        var repoRoot = FindRepoRoot();


        Port = ProcessRunner.GetFreePort();
        Process = ProcessRunner.StartGrpcServer(repoRoot, Port);

        Process = ProcessRunner.StartGrpcServer(repoRoot, Port);
        await ProcessRunner.WaitForPortAsync("localhost", Port, TimeSpan.FromSeconds(20), Process);

    }

    public async Task DisposeAsync()
    {
        if (Process is not null)
            await Process.DisposeAsync();
    }
}
