using Customer.Service.Enums;

namespace Customer.Service.Models
{
    public class CustomerStatusUpdateRequest
    {      
        public CustomerStatus? Status { get; set; }
    }
}
