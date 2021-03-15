using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.API.Services
{
    public class UserTokenService
    {

        readonly IConfiguration _configuration;
        string Issuer = string.Empty;
        string Audience = string.Empty;
        string Secret = string.Empty;
        int DurationInMinutes = 0;
        public UserTokenService(IConfiguration configuration)
        {
            this._configuration = configuration;

            Issuer = _configuration["JWTSettings:Issuer"];
            Audience = _configuration["JWTSettings:Audience"];
            Secret = _configuration["JWTSettings:Key"];
            DurationInMinutes = Convert.ToInt32(_configuration["JWTSettings:DurationInMinutes"]);
        }
        public string GenUserToken(string username, string role)
        {
           

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.NameIdentifier,username),
                new Claim(ClaimTypes.HomePhone,"15803807012")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(Issuer, Audience, claims, expires: DateTime.Now.AddMinutes(DurationInMinutes), signingCredentials: credentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return token;
        }
    }
}
