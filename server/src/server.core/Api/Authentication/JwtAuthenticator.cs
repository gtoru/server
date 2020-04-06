using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using server.core.Application;
using server.core.Infrastructure;

namespace server.core.Api.Authentication
{
    public class JwtAuthenticator : IAuthenticator
    {
        private readonly int _daysToExpire;
        private readonly byte[] _key;

        public JwtAuthenticator(IConfiguration configuration)
        {
            _key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
            _daysToExpire = int.Parse(configuration["Jwt:DaysToExpire"]);
        }

        public async Task<string> AuthenticateAsync(IUnitOfWork unitOfWork, string email, string password)
        {
            var user = await AuthenticationManager.AuthenticateAsync(unitOfWork, email, password);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.AccessLevel.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(_daysToExpire),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
