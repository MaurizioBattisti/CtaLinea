namespace CtaLineaWebApi.Auth
{
    public class JwtOptions
    {
        public bool ValidateIssuerSigningKey { get; set; }
        public string IssuerSigningKey { get; set; } = "A Very Long default key with @#/ and other  .:;_+-! and some numbers 123987555 555 0152 Key";
        public bool ValidateIssuer { get; set; } = true;
        public string ValidIssuer { get; set; }
        public bool ValidateAudience { get; set; } = true;
        public string ValidAudience { get; set; }
        public bool RequireExpirationTime { get; set; }
        public bool ValidateLifetime { get; set; } = true;

        public int TokenDurationHours { get; set; } = 12;
    }
}

