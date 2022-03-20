using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElementsTheAPI.Filters
{
    public class JwtAuth : IJwtAuth
    {
        private readonly string key;
        public JwtAuth(string key)
        {
            this.key = key;
        }
        public string Authentication(string username, string playerID)
        {

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);

            var header = new JwtHeader(credentials);

            DateTime expiry = DateTime.UtcNow.AddMinutes(1);

            int ts = (int)(expiry - new DateTime(1970, 1, 1)).TotalSeconds;

            var payload = new JwtPayload
            {
                { "sub", "EtG" },
                { "name", username },
                { "playerID", playerID },
                { "exp", ts },
                { "iss", "http://localhost:5000" },
                { "aud", "http://localhost:5000" }
            };

            var secToken = new JwtSecurityToken(header, payload);

            var tokenHandler = new JwtSecurityTokenHandler();

            // 5. Return Token from method
            return tokenHandler.WriteToken(secToken);
        }
    }
}
