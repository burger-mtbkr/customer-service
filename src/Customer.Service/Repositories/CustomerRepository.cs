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

        public IEnumerable<CustomerModel> GetAllCustomers() => _collection.AsQueryable();

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

        public async Task<bool> DeleteCustomerAsync(CustomerModel customer) =>  await _collection.DeleteOneAsync(customer.Id);
    }
}
