using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface ILeadRepository
    {
        IEnumerable<LeadModel> GetLeads(string customerId);
        LeadModel? GetLeadByid(string id);
        Task<LeadModel> SaveLeadAsync(LeadModel model);
    }
}
