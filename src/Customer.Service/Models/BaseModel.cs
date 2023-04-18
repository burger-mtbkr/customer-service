namespace Customer.Service.Models
{
    public record BaseModel
    {
        public string? Id { get; set; }
        public DateTime? CreatedDateUtc { get; set; }
    }
}
