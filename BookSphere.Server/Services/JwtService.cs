using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookSphere.IServices;
using BookSphere.Models;
using Microsoft.IdentityModel.Tokens;

namespace BookSphere.Services;

public class JwtService : IJwtService
{
          public readonly IConfiguration _configuration;

          public JwtService(IConfiguration configuration)
          {
                    _configuration = configuration;
          }
          public string GenerateToken(User user)
          {
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new[]
                    {
                              new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                              new Claim(JwtRegisteredClaimNames.Email, user.EmailAddress),
                              new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                              new Claim(ClaimTypes.Name, user.FullName),
                              new Claim(ClaimTypes.Role, user.Role)
                    };

                    var token = new JwtSecurityToken(
                              issuer: _configuration["Jwt:Issuer"],
                              audience: _configuration["Jwt:Audience"],
                              claims: claims,
                              expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"])),
                              signingCredentials: creds
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
          }
}
