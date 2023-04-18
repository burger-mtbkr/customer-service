using Customer.Service.Enums;

namespace Customer.Service.Models
{
    public record LeadModel: BaseModel
    {
        public string CustomerId { get; set; }
        public LeadStatus Status { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
    }
}
