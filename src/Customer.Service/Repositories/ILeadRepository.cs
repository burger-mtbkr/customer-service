﻿using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface ILeadRepository
    {
        IEnumerable<LeadModel> GetLeads(string customerId);
        LeadModel? GetLeadById(string id);
        Task<LeadModel> SaveLeadAsync(LeadModel model);
        Task<bool> DeleteAllAsync(string customerId);
    }
}
