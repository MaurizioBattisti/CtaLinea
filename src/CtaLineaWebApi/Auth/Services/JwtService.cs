using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CtaLineaWebApi.Auth.Model;
using System.Collections.Generic;
using System;

namespace CtaLineaWebApi.Auth.Services
{

    public class JwtService
        : IJwtService
    {
        private readonly JwtOptions _options;
        public JwtService(
            IOptions<JwtOptions> options
            )
        {
            _options = options.Value;
        }

        public JwtResult GenTokenkey(
            string userName,
            string description,
            string eMail,
            Guid? associateId,
            IEnumerable<string> roles,
            bool mustChangePAssword = false
            )
        {
            try
            {
                var TokenCreated = DateTime.UtcNow;
                var TokenExpire = TokenCreated.AddHours(_options.TokenDurationHours);
                var TokenValidaty = TokenCreated.TimeOfDay;
                // Get secret key
                byte[] key = System.Text.Encoding.ASCII.GetBytes(_options.IssuerSigningKey);
                List<Claim> TokenClaims = new List<Claim>();
                TokenClaims.Add(new Claim(ClaimTypes.Name, userName));
                TokenClaims.Add(new Claim(ClaimTypes.Email, eMail));
                // TokenClaims.Add(new Claim(ClaimTypes.NameIdentifier, userName)); //Id.ToString()));

                // aggiunge i claim custom
                TokenClaims.Add(new Claim(Constants.ClaimType_AssociateId, associateId?.ToString() ?? string.Empty));

                TokenClaims.Add(new Claim(ClaimTypes.Expiration, TokenExpire.ToString("MMM ddd dd yyyy HH:mm:ss tt")));

                TokenClaims.Add(new Claim(ClaimTypes.UserData, mustChangePAssword.ToString()));

                foreach (var r in roles)
                {
                    TokenClaims.Add(
                        new Claim(ClaimTypes.Role, r)
                        );
                }

                var JWToken = new JwtSecurityToken(
                    issuer: _options.ValidIssuer,
                    audience: _options.ValidAudience,
                    claims: TokenClaims,
                    notBefore: TokenCreated,
                    expires: TokenExpire,
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256)
                    );
                string tkn = new JwtSecurityTokenHandler().WriteToken(JWToken);
                var jwt = new JwtResult()
                {
                    Token = tkn,
                    Expiration = TokenExpire
                };
                return jwt;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}