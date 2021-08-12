using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _Key;

        public TokenService(IConfiguration config)
        {
            _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWToken"]));
        }

        public string CreateToken(AppUser user)
        {
            var claim = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.NameId, user.UserName)
            };

            var cred = new SigningCredentials(_Key,SecurityAlgorithms.HmacSha512);

            var tokendescriptor = new SecurityTokenDescriptor{
                Subject = new ClaimsIdentity(claim),
                Expires = System.DateTime.Now.AddSeconds(10),
                SigningCredentials = cred
            };

            var tokenhandler =  new JwtSecurityTokenHandler();
            var token = tokenhandler.CreateToken(tokendescriptor);

            return tokenhandler.WriteToken(token);

        }
    }
}