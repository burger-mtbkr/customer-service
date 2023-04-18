using Customer.Service.Enums;

namespace Customer.Service.Models
{
    public record CustomerModel: BaseModel
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Company { get; init; }
        public string Email { get; init; }
        public string PhoneNumber { get; init; }
        public CustomerStatus? Status { get; set; }
    }
}
