using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ContactManager.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config) => _config = config;

    public sealed record LoginRequest(string Username, string Password);

    public sealed record LoginResponse(string AccessToken, string TokenType, int ExpiresInSeconds);

    [HttpPost("login")]
    public ActionResult<LoginResponse> Login([FromBody] LoginRequest req)
    {
        // Hardcoded user for Phase 3
        // Later: replace with real user store + password hashing.
        if (req.Username != "admin" || req.Password != "password")
            return Unauthorized(new { message = "Invalid credentials" });

        var jwt = _config.GetSection("Jwt");
        var issuer = jwt["Issuer"]!;
        var audience = jwt["Audience"]!;
        var key = jwt["Key"]!;
        var expiresMinutes = int.Parse(jwt["ExpiresMinutes"] ?? "60");

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, req.Username),
            new(ClaimTypes.Name, req.Username),
            new(ClaimTypes.Role, "admin") // or "user"
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new LoginResponse(
            AccessToken: tokenString,
            TokenType: "Bearer",
            ExpiresInSeconds: (int)TimeSpan.FromMinutes(expiresMinutes).TotalSeconds
        ));
    }
}
