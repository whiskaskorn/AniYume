using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Models;

namespace Handlers;

public class JwtHandler
{
    IConfiguration _configuration;
    public JwtHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    private string tokenSearch()
    {
        return _configuration.GetValue<string>("ApiKeys:JwtSecretKey") ?? throw new InvalidOperationException("Не задано JwtSecretKey!");
    }
    public string GenerateToken(User user)
    {
        var jwtSecretKey = tokenSearch();
        List<Claim> claims = new List<Claim>
        {
            new Claim("Id",Convert.ToString(user.Id)),
            new Claim("Username", user.Username!),
            new Claim("Email", user.Email!),
            new Claim(ClaimTypes.Role,user.Roles![0])
        };
        var jwtToken = new JwtSecurityToken(
            expires: DateTime.UtcNow.AddHours(1),
            claims: claims,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
                SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
    
    public void ConfigureToken(IServiceCollection serviceCollection)
    {
        var jwtSecretKey = tokenSearch();
        serviceCollection.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
            o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey))
                };
            }
        );
    }
}