using System;

namespace CtaLineaWebApi.Auth.Model
{
    public class JwtResult
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
    }
}
