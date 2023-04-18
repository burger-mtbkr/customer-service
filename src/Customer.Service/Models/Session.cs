namespace Customer.Service.Models
{
    public record Session: BaseModel
    {
        public string Token { get; init; }
        public string UserId { get; init; }
        public string UserEmail { get; init; }
        public DateTime Expiry { get; init; }
    }
}
