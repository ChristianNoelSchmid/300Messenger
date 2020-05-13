using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _300Messenger.Authentication.Services
{
    public class TokenBuilder : ITokenBuilder
    {
        public string BuildToken(string email)
        {
            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    "what1sth3k1nd3stth1ngthatha53v3rb33nsa1dt0y0uf0rm31t1sthat1msaf3"
                )
            );
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, email)
            };
            
            var jwt = new JwtSecurityToken(claims: claims, signingCredentials: signingCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
        }
    }
}