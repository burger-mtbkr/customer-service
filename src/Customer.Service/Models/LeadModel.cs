using Customer.Service.Enums;

namespace Customer.Service.Models
{
    public record LeadModel: BaseModel
    {
        public string CustomerId { get; init; }
        public LeadStatus Status { get; init; }
        public string Name { get; init; }
        public string Source { get; init; }
    }
}
