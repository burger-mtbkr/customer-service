using Customer.Service.Exceptions;
using Customer.Service.Models;
using Customer.Service.Repositories;

namespace Customer.Service.Services
{
    public class LeadsService: ILeadsService
    {
        private readonly ILeadRepository _leadRepository;

        public LeadsService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public LeadModel? GetLeadById(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var lead = _leadRepository.GetLeadById(id);
            if(lead == null) throw new LeadNotFoundException($"Lead not found for id {id}");
            return lead;
        }

        public IEnumerable<LeadModel> GetLeads(string customerId)
        {
            // TODO consider adding validation checks for when no customer exists for the provided id
            // (or at least add logging for it)
            if(string.IsNullOrEmpty(customerId)) throw new ArgumentNullException(nameof(customerId));
            return _leadRepository.GetLeads(customerId);
        }

        public async Task<LeadModel> CreateLeadAsync(LeadModel model)
        {
            ValidateLeadModel(model);

            model.Id = Guid.NewGuid().ToString();
            model.CreatedDateUtc = DateTime.UtcNow;

            return await _leadRepository.SaveLeadAsync(model);
        }

        public async Task<bool> UpdateLeadAsync(string id, LeadModel model)
        {
            ValidateLeadModel(model);

            // make sure the lead exists
            GetLeadById(id);
            await _leadRepository.SaveLeadAsync(model);
            return true;
        }

        /// <summary>
        /// Worth noting I would normally use a type of action filter
        /// to manage valdiation for a particular entity type but in the interest of time opeted for a more manual approach. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private bool ValidateLeadModel(LeadModel model)
        {
            if(model == null) throw new ArgumentNullException(nameof(model));
            if(string.IsNullOrEmpty(model.Source)) throw new ArgumentException($"{nameof(model.Source)} is required");
            if(string.IsNullOrEmpty(model.Name)) throw new ArgumentException($"{nameof(model.Name)} is required");
            if(string.IsNullOrEmpty(model.CustomerId)) throw new ArgumentException($"{nameof(model.CustomerId)} is required");
            if(!model.Status.HasValue) throw new ArgumentException($"{nameof(model.Status)} is required");
            return true;
        }
    }
}
