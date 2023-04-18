using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<CustomerModel> GetAllCustomers();
        CustomerModel? GetCustomerByID(string id);
        Task<CustomerModel> SaveCustomerAsync(CustomerModel model);
        Task<bool> DeleteCustomerAsync(CustomerModel customer);
    }
}
