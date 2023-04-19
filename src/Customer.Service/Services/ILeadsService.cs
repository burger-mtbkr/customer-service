using Customer.Service.Models;

namespace Customer.Service.Services
{
    public interface ILeadsService
    {
        IEnumerable<LeadModel> GetLeads(string customerId);
        LeadModel? GetLeadById(string id);
        Task<LeadModel> CreateLeadAsync(LeadModel model);
        Task<bool> UpdateLeadAsync(string id, LeadModel model);
    }
}
