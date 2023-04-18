using Customer.Service.Exceptions;
using Customer.Service.Models;
using Customer.Service.Repositories;

namespace Customer.Service.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _repository;

        public CustomerService(ICustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerModel> CreateCustomer(CustomerModel model)
        {
            // Worth noting I would normally use a type of action filter
            // to manage valdiation for a particular entity type but in the interest of time opeted for a more manual approach. 
            if(model == null) throw new ArgumentNullException(nameof(model));
            if(string.IsNullOrEmpty(model.Email)) throw new ArgumentException($"{nameof(model.Email)} is required");
            if(string.IsNullOrEmpty(model.FirstName)) throw new ArgumentException($"{nameof(model.FirstName)} is required");
            if(string.IsNullOrEmpty(model.LastName)) throw new ArgumentException($"{nameof(model.LastName)} is required");
            if(string.IsNullOrEmpty(model.PhoneNumber)) throw new ArgumentException($"{nameof(model.PhoneNumber)} is required");
            if(string.IsNullOrEmpty(model.Company)) throw new ArgumentException($"{nameof(model.PhoneNumber)} is required");
            if(!model.Status.HasValue) throw new ArgumentException($"{nameof(model.Status)} is required");

            model.Id = Guid.NewGuid().ToString();
            model.CreatedDateUtc = DateTime.UtcNow;

            return await _repository.SaveCustomerAsync(model);
        }

        public IEnumerable<CustomerModel> GetAllCustomers() => _repository.GetAllCustomers();

        public CustomerModel GetCustomerByID(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var user = _repository.GetCustomerByID(id);
            if(user == null) throw new CustomerNotFoundException($"Customer not found for id {id}");
            return user;
        }

        public async Task<bool> UpdateCustomerAsync(string id, CustomerModel model)
        {
            if(model == null) throw new ArgumentNullException(nameof(model));
            // Get the user to ensure it exists
            GetCustomerByID(id);
            await _repository.SaveCustomerAsync(model);
            return true;
        }

        public async Task<bool> UpdateCustomerAsync(string id, CustomerStatusUpdateRequest model)
        {
           var customer = GetCustomerByID(id);
           if(customer == null) throw new CustomerNotFoundException($"Customer not found for id {id}");
           customer.Status = model.Status;
           await _repository.SaveCustomerAsync(customer);
           return true;
        }

        public async Task<bool> DeleteCustomer(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var customer = GetCustomerByID(id);
            var result = await _repository.DeleteCustomerAsync(customer);
            return result;
        }
    }
}
