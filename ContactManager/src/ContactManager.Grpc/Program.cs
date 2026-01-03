using ContactManager.Data;
using ContactManager.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();

var provider = builder.Configuration["Storage:Provider"] ?? "sqlite";

//builder.Services.AddDbContext<ContactManagerDbContext>(options =>
//{
//    if (provider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase))
//        options.UseSqlServer(builder.Configuration.GetConnectionString("ContactManagerDb"));
//    else
//        options.UseSqlite(builder.Configuration.GetConnectionString("ContactManagerDb")
//            ?? "Data Source=contactmanager.grpc.db");
//});


builder.Services.AddDbContext<ContactManagerDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("ContactManagerDb")
             ?? "Data Source=contactmanager.grpc.db";
    options.UseSqlite(cs);
});

var app = builder.Build();

app.MapGrpcService<ContactsGrpcService>();
app.MapGet("/", () => "gRPC server running.");

app.Run();
