using Facturi.Modele;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Facturi.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public bool ValidateCredentials(string username, string password)
    {
        // Exemplu simplu: pe un monolit modular real ar trebui sa foloseasca un user store/DB encryptat
        return !string.IsNullOrWhiteSpace(username)
            && !string.IsNullOrWhiteSpace(password)
            && username == "admin"
            && password == "admin123";
    }

    public string GenerateJwtToken(string username)
    {
        var jwtSecret = _configuration.GetValue<string>("JwtSecret") ?? "AceastaEsteOCheieJWTFoarteSigura123!";
        var jwtIssuer = _configuration.GetValue<string>("JwtIssuer") ?? "FacturiAPI";
        var jwtAudience = _configuration.GetValue<string>("JwtAudience") ?? "FacturiClients";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
