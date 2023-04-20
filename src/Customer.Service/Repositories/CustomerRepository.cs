using Customer.Service.Exceptions;
using Customer.Service.Models;
using JsonFlatFileDataStore;

namespace Customer.Service.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly IDocumentCollection<CustomerModel> _collection;
        public CustomerRepository(IDocumentCollection<CustomerModel> collection)
        {
            _collection = collection;
        }

        public IEnumerable<CustomerModel> GetAllCustomers(string searchText)
        {
            var collection = _collection.AsQueryable();

            if(string.IsNullOrEmpty(searchText.ToLower())) return collection;
            return collection.Where(c =>
            c.FirstName.ToLower().StartsWith(searchText.ToLower()) ||
            c.LastName.ToLower().StartsWith(searchText.ToLower()) ||
            c.Company.ToLower().StartsWith(searchText.ToLower()) ||
            c.Email.ToLower().StartsWith(searchText.ToLower()) ||
            c.PhoneNumber.ToLower().StartsWith(searchText.ToLower()));
        }

        public CustomerModel? GetCustomerByID(string id)
        {
            var collection = _collection.AsQueryable();
            return collection.FirstOrDefault(c => c.Id == id);
        }

        public async Task<CustomerModel> SaveCustomerAsync(CustomerModel model)
        {
            await _collection.ReplaceOneAsync(model.Id, model, true);
            return model;
        }

        public async Task<bool> DeleteCustomerAsync(CustomerModel customer) => await _collection.DeleteOneAsync(customer.Id);
    }
}

