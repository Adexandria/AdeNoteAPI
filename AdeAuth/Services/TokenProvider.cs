using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace AdeAuth.Services
{
     class TokenProvider : ITokenProvider
     {
        public void GetTokenEncryptionKey(string tokenKey)
        {
            _tokenKey = tokenKey 
             ?? throw new NullReferenceException("Invalid token key");
        }

        public string GenerateRefreshToken(int tokenSize)
        {
            var randomNumber = new byte[tokenSize];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);

            var refreshtoken = Convert.ToBase64String(randomNumber);
            return refreshtoken;
        }

        public string GenerateToken(Dictionary<string, object> claims, int timeInMinutes)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = claims,
                Expires = DateTime.UtcNow.AddMinutes(timeInMinutes),
                SigningCredentials = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(securityTokenDescriptor);
            string token = tokenHandler.WriteToken(securityToken);

            return token;
        }

        public IList<string> VerifyToken(string token, bool verifyParamter, params string[] claimTypes)
        {
            TokenValidationParameters tokenValidationParameters = GetTokenValidationParameters(verifyParamter);

            JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
            List<string> claimValues = new();

            try
            {
                var claims = jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

                foreach (var claimType in claimTypes)
                {
                    var claimValue = claims.Claims.Where(s=>s.Type == claimType)
                        .Select(s=>s.Value).FirstOrDefault();

                    if(claimValue == null)
                        continue;

                    claimValues.Add(claimValue);
                }

                return claimValues;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }


        private SecurityKey GetSymmetricSecurityKey()
        {
            byte[] symmetricKey = Encoding.UTF8.GetBytes(_tokenKey);
            return new SymmetricSecurityKey(symmetricKey);
        }

        private TokenValidationParameters GetTokenValidationParameters(bool verifyParameter)
        {
            return new TokenValidationParameters()
            {
                ValidateIssuer = verifyParameter,
                ValidateAudience = verifyParameter,
                IssuerSigningKey = GetSymmetricSecurityKey()
            };
        }

        private string _tokenKey;
    }

}
