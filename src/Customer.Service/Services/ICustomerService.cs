using Customer.Service.Models;

namespace Customer.Service.Services
{
    public interface ICustomerService
    {
        IEnumerable<CustomerModel> GetAllCustomers(string search);
        CustomerModel GetCustomerByID(string id);
        Task<CustomerModel> CreateCustomerAsync(CustomerModel model);
        Task<bool> UpdateCustomerAsync(string id, CustomerModel model);
        Task<bool> UpdateCustomerStatusAsync(string id, CustomerStatusUpdateRequest model);
        Task<bool> DeleteCustomerAsync(string id);
    }
}
