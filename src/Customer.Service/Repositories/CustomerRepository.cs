using Customer.Service.Enums;
using Customer.Service.Models;
using JsonFlatFileDataStore;
using System.Globalization;

namespace Customer.Service.Repositories
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly IDocumentCollection<CustomerModel> _collection;
        public CustomerRepository(IDocumentCollection<CustomerModel> collection)
        {
            _collection = collection;
        }

        public IEnumerable<CustomerModel> GetAllCustomers(GetUsersRequest request)
        {
            var collection = _collection.AsQueryable();


            var lowerCaseSearchText = request?.SearchText?.ToLower();

            // Step 1: Filter by search text (or return all data if searchText is empty)
            var filteredList = string.IsNullOrEmpty(lowerCaseSearchText)
                ? collection
                : collection.Where(c =>
            c.FirstName.ToLower().StartsWith(lowerCaseSearchText) ||
            c.LastName.ToLower().StartsWith(lowerCaseSearchText) ||
            c.Company.ToLower().StartsWith(lowerCaseSearchText) ||
            c.Email.ToLower().StartsWith(lowerCaseSearchText) ||
            c.PhoneNumber.ToLower().StartsWith(lowerCaseSearchText));

            // Step 2: Filter by status
            var statusToFilterOn = StatusTextToEnum(request.StatusFilter);
            if(statusToFilterOn.HasValue)
            {
                filteredList = filteredList.Where(item => item.Status == statusToFilterOn).ToList();
            }

            if(string.IsNullOrEmpty(request.SortBy)) return filteredList;

            return filteredList?.Any() == true ? SortSearchData(filteredList, request.SortBy, request.SortDirection) : new List<CustomerModel>();
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

        /// <summary>
        /// I would prefer to have these status' in the datastore rather than use a enum but this was simple faster for the purose for the assessment. 
        /// </summary>
        /// <param name="statusFilter"></param>
        /// <returns></returns>
        private CustomerStatus? StatusTextToEnum(int? statusFilter = null)
        {
            if(statusFilter.HasValue)
            {
                if(statusFilter == 0) return CustomerStatus.ACTIVE;
                if(statusFilter == 1) return CustomerStatus.LEAD;
                if(statusFilter == 2) return CustomerStatus.NON_ACTIVE;
            }

            return null;
        }

        /// <summary>
        /// This is very ugle but will work for now. 
        /// I dont like using reflection due to performance especially for things liek searching on a list response
        /// which could potentialy be massive. 
        /// Again using stored prcedures or elastic search would make this much easier
        /// </summary>
        /// <param name="data"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private IEnumerable<CustomerModel> SortSearchData(IEnumerable<CustomerModel> data, string sortBy, string sortDirection)
        {
            // Step 3: Sort by field and direction

            // match the search field to the Proeprty by ight case
            var type = typeof(CustomerModel);
            var proeprties = type.GetProperties();
            var sortField = proeprties.FirstOrDefault(prop => string.Equals(prop.Name, sortBy, StringComparison.OrdinalIgnoreCase))?.Name;

            if(string.IsNullOrEmpty(sortField))
            {
                if(sortDirection == "desc")
                {
                    return data.OrderByDescending(c => c.FirstName);

                }

                return data.OrderBy(c => c.FirstName);
            }

            if(sortDirection == "desc")
            {
                return data.OrderByDescending(item => MatchPropertyName(item, sortField)).ToList();

            }

            return data.OrderBy(item => MatchPropertyName(item, sortField)).ToList();

        }

        private object? MatchPropertyName(CustomerModel item, string sortBy) => item.GetType().GetProperty(sortBy).GetValue(item);
    }
}

