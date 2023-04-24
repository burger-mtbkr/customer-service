using Customer.Service.Enums;

namespace Customer.Service.Models
{
    public record CustomerModel: BaseModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public CustomerStatus? Status { get; set; }
        public int LeadCount { get; set; }
    }
}
