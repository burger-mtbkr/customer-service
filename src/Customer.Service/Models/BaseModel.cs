namespace Customer.Service.Models
{
    public record BaseModel
    {
        public string Id { get; init; }
        public DateTime CreatedDateUtc { get; set; }
    }
}
