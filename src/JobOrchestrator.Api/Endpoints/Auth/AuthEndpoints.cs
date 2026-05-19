using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JobOrchestrator.Api.Endpoints.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/auth")
            .WithTags("Authentication");

        group.MapGet("/token", Login)
            .AllowAnonymous()
            .WithName("Login")
            .WithDescription("Endpoint for user login and JWT token generation.");
    }

    public static async Task<IResult> Login(IConfiguration configuration)
    {
        byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, "RecrutadorTeste"),
                new Claim(ClaimTypes.Role, "Admin")
                ]),
            Expires = DateTime.UtcNow.AddHours(2),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Results.Ok(new { AccessToken = tokenHandler.WriteToken(token) });
    }
}