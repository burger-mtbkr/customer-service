using Customer.Service.Exceptions;
using Customer.Service.Models;
using Customer.Service.Repositories;
using System.Globalization;

namespace Customer.Service.Services
{
    public class CustomerService: ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly ILeadsService _leadsService;

        public CustomerService(ICustomerRepository repository, ILeadsService leadsService)
        {
            _repository = repository;
            _leadsService = leadsService;
        }

        public async Task<CustomerModel> CreateCustomerAsync(CustomerModel model)
        {
            ValidateCustomerModel(model);

            model.Id = Guid.NewGuid().ToString();
            model.CreatedDateUtc = DateTime.UtcNow;

            return await _repository.SaveCustomerAsync(model);
        }

        public IEnumerable<CustomerModel>? GetAllCustomers(GetCustomerRequest request)
        {
            var customers = _repository.GetAllCustomers(request);

            if(customers?.Any() == true)
            {
                foreach(var c in customers)
                {
                    var leads = _leadsService.GetLeads(c.Id!);
                    if(leads?.Any() == true)
                    {
                        c.LeadCount = leads.Count();
                    }
                }
                return customers;
            }
            return new List<CustomerModel>();
        }

        public CustomerModel GetCustomerByID(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var customer = _repository.GetCustomerByID(id);
            if(customer == null) throw new CustomerNotFoundException($"Customer not found for id {id}");
            var leads = _leadsService.GetLeads(customer.Id!);
            if(leads?.Any() == true)
            {
                customer.LeadCount = leads.Count();
            }

            return customer;
        }

        public async Task<bool> UpdateCustomerAsync(string id, CustomerModel model)
        {
            ValidateCustomerModel(model);
            // Get the user to ensure it exists
            GetCustomerByID(id);
            await _repository.SaveCustomerAsync(model);
            return true;
        }

        public async Task<bool> UpdateCustomerStatusAsync(string id, CustomerStatusUpdateRequest model)
        {
            if(!model.Status.HasValue) throw new ArgumentException($"{nameof(model.Status)} is required");

            var customer = GetCustomerByID(id);
            customer.Status = model.Status;
            await _repository.SaveCustomerAsync(customer);
            return true;
        }

        public async Task<bool> DeleteCustomerAsync(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var customer = GetCustomerByID(id);

            var leads = _leadsService.GetLeads(customer.Id!);
            if(leads?.Any() == true)
            {
               foreach( var lead in leads)
                {
                    await _leadsService.DeleteAllAsync(customer.Id!);
                }
            }

            var result = await _repository.DeleteCustomerAsync(customer);
            return result;
        }

        /// <summary>
        /// Worth noting I would normally use a type of action filter
        /// to manage valdiation for a particular entity type but in the interest of time opeted for a more manual approach. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateCustomerModel(CustomerModel model)
        {
            if(model == null) throw new ArgumentNullException(nameof(model));
            if(string.IsNullOrEmpty(model.Email)) throw new ArgumentException($"{nameof(model.Email)} is required");
            if(string.IsNullOrEmpty(model.FirstName)) throw new ArgumentException($"{nameof(model.FirstName)} is required");
            if(string.IsNullOrEmpty(model.LastName)) throw new ArgumentException($"{nameof(model.LastName)} is required");
            if(string.IsNullOrEmpty(model.PhoneNumber)) throw new ArgumentException($"{nameof(model.PhoneNumber)} is required");
            if(string.IsNullOrEmpty(model.Company)) throw new ArgumentException($"{nameof(model.Company)} is required");
            if(!model.Status.HasValue) throw new ArgumentException($"{nameof(model.Status)} is required");

            return true;
        }
    }
}
