using ContactManager.Data;
using ContactManager.Grpc.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ Force HTTP/2 (h2c) on localhost:5055 for gRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5055, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
    });
});

// gRPC
builder.Services.AddGrpc();

// EF Core (SQLite file owned by gRPC service)
builder.Services.AddDbContext<ContactManagerDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("ContactManagerDb")
             ?? "Data Source=contactmanager.grpc.db";
    options.UseSqlite(cs);
});

var app = builder.Build();

app.MapGrpcService<ContactsGrpcService>();

// Optional: a simple HTTP endpoint for sanity (this will be HTTP/2 now)
app.MapGet("/", () => "gRPC server running.");

app.Run();
