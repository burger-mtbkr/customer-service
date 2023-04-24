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

        public IEnumerable<CustomerModel>? GetAllCustomers(GetCustomerRequest request)
        {
            var customers = _repository.GetAllCustomers(request);

            if(customers?.Any() == true)
            {
                foreach(var c in customers)
                {
                    c.LeadCount = GetLeadCount(c.Id);
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
            customer.LeadCount = GetLeadCount(id);
            return customer;
        }

        public async Task<CustomerModel> CreateCustomerAsync(CustomerModel model)
        {
            ValidateCustomerModel(model);

            // TODO: consider what makes a customer unique.  SHould we disallow adding a new customer if one already exists
            // with the same email for the same company? I hav enot applied this rule for now but worth thinking about.             

            model.Id = Guid.NewGuid().ToString();
            model.CreatedDateUtc = DateTime.UtcNow;

            return await _repository.SaveCustomerAsync(model);
        }

        public async Task<bool> UpdateCustomerAsync(string id, CustomerModel model)
        {
            ValidateCustomerModel(model);

            var customer = GetCustomerByID(id);
            if(customer != null)
            {
                customer.Company = model.Company;
                customer.PhoneNumber = model.PhoneNumber;
                customer.FirstName = model.FirstName;
                customer.LastName = model.LastName;
                customer.Email = model.Email;
                customer.Status = model.Status;

                await _repository.SaveCustomerAsync(customer);
            }
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
                await _leadsService.DeleteAllAsync(customer.Id!);
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

        private int GetLeadCount(string customerId)
        {
            var leads = _leadsService.GetLeads(customerId);
            return leads.Count();
        }
    }
}
