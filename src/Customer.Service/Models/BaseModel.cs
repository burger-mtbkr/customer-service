namespace Customer.Service.Models
{
    public record BaseModel
    {
        public string Id { get; init; }
        public DateTime CreatedDate { get; set; }
    }
}
