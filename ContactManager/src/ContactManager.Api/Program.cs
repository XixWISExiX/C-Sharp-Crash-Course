using System.Text;
using ContactManager.Api.Contacts;
using ContactManager.Api.Contacts.Query;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

static string? GetArg(string[] args, string name)
{
    for (int i = 0; i < args.Length - 1; i++)
        if (string.Equals(args[i], name, StringComparison.OrdinalIgnoreCase))
            return args[i + 1];

    foreach (var a in args)
        if (a.StartsWith(name + "=", StringComparison.OrdinalIgnoreCase))
            return a[(name.Length + 1)..];

    return null;
}

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
}

// gRPC client address (arg > config > default)
var grpcFromArg = GetArg(args, "--grpc_link");
var grpcUrl = grpcFromArg
              ?? builder.Configuration["Grpc:ContactsUrl"]
              ?? "http://localhost:5055";

builder.Services.AddGrpcClient<ContactManager.Grpc.Contacts.ContactsService.ContactsServiceClient>(o =>
{
    o.Address = new Uri(grpcUrl);
});


builder.Services.AddControllers();

// Strategy
builder.Services.AddSingleton<IContactQueryStrategy, StartsWithStrategy>();
builder.Services.AddSingleton<IContactQueryStrategy, ContainsStrategy>();

// Decorator (cache)
builder.Services.AddMemoryCache();
builder.Services.AddScoped<GrpcContactsClient>();
builder.Services.AddScoped<IContactsClient>(sp =>
{
    var inner = sp.GetRequiredService<GrpcContactsClient>();
    var cache = sp.GetRequiredService<IMemoryCache>();
    return new CachedContactsClient(inner, cache);
});

// JWT config
var jwt = builder.Configuration.GetSection("Jwt");
var issuer = jwt["Issuer"];
var audience = jwt["Audience"];
var key = jwt["Key"];

// AuthN
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,

            ValidateAudience = true,
            ValidAudience = audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// AuthZ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("admin"));
});

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
