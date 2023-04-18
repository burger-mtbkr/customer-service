namespace Customer.Service.Infrastructure.Auth
{
    public record TokenSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryHours { get; init; }
        public bool RequireHttpsMetadata { get; set; }
        public bool SaveToken { get; set; }
        public bool ValidateIssuer { get; set; }
        public bool ValidateAudience { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool RequireExpirationTime { get; set; }
    }
}
