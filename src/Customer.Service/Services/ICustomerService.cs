using Customer.Service.Models;

namespace Customer.Service.Services
{
    public interface ICustomerService
    {
        IEnumerable<CustomerModel> GetAllCustomers();
        CustomerModel GetCustomerByID(string id);
        Task<CustomerModel> CreateCustomer(CustomerModel model);
        Task<bool> UpdateCustomerAsync(string id, CustomerModel model);
        Task<bool> UpdateCustomerAsync(string id, CustomerStatusUpdateRequest model);
        Task<bool> DeleteCustomer(string id);
    }
}
