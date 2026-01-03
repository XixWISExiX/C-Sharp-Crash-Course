using System.Net;
using System.Net.Sockets;
using ContactManager.Data;
using ContactManager.Grpc.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ContactManager.Tests.Infrastructure;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // API DB (keeps API startup from touching disk)
    private SqliteConnection? _apiConnection;

    // gRPC host + DB
    private IHost? _grpcHost;
    private SqliteConnection? _grpcConnection;
    private string? _grpcUrl;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // 1) Start an in-process gRPC server (HTTP/2) for the API to call.
        EnsureGrpcServerStarted();

        // 2) Force the API to use the test gRPC server address.
        builder.ConfigureAppConfiguration((ctx, cfg) =>
        {
            cfg.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Grpc:ContactsUrl"] = _grpcUrl
            });
        });

        // 3) Keep your API DbContext override (in case API still migrates/uses EF at startup).
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services
                .SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ContactManagerDbContext>));

            if (dbContextDescriptor is not null)
                services.Remove(dbContextDescriptor);

            _apiConnection = new SqliteConnection("DataSource=:memory:");
            _apiConnection.Open();

            services.AddDbContext<ContactManagerDbContext>(options =>
            {
                options.UseSqlite(_apiConnection);
            });

            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ContactManagerDbContext>();
            db.Database.Migrate();
        });
    }

    private void EnsureGrpcServerStarted()
    {
        if (_grpcHost is not null) return;

        var port = GetFreePort();
        _grpcUrl = $"http://localhost:{port}";

        // Keep one open connection so SQLite :memory: persists for the gRPC host lifetime
        _grpcConnection = new SqliteConnection("DataSource=:memory:");
        _grpcConnection.Open();

        var appBuilder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Testing"
        });

        // Force HTTP/2 (h2c) for gRPC
        appBuilder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(port, listen =>
            {
                listen.Protocols = HttpProtocols.Http2;
            });
        });

        appBuilder.Services.AddGrpc();

        appBuilder.Services.AddDbContext<ContactManagerDbContext>(opt =>
        {
            opt.UseSqlite(_grpcConnection);
        });

        var app = appBuilder.Build();

        // Apply migrations for gRPC-owned DB
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ContactManagerDbContext>();
            db.Database.Migrate();
        }

        // Map gRPC service
        app.MapGrpcService<ContactsGrpcService>();

        // Start the gRPC host
        app.Start();

        // WebApplication implements IHost, so we can store/dispose it as IHost
        _grpcHost = app;
    }

    private static int GetFreePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing) return;

        try
        {
            _grpcHost?.Dispose();
            _grpcHost = null;
        }
        catch { /* ignore */ }

        _grpcConnection?.Dispose();
        _grpcConnection = null;

        _apiConnection?.Dispose();
        _apiConnection = null;
    }
}
