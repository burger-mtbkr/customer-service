﻿using Customer.Service.Models;
using JsonFlatFileDataStore;
using System.Reflection;

namespace Customer.Service.Repositories
{
    public class LeadRepository: ILeadRepository
    {
        private readonly IDocumentCollection<LeadModel> _collection;
        public LeadRepository(IDocumentCollection<LeadModel> collection)
        {
            _collection = collection;
        }

        public IEnumerable<LeadModel> GetLeads(string customerId)
        {
            var collection = _collection.AsQueryable();
            return collection.Where(c => c.CustomerId == customerId);
        }

        public LeadModel? GetLeadById(string id)
        {
            var collection = _collection.AsQueryable();
            return collection.FirstOrDefault(c => c.Id == id);
        }     

        public async Task<LeadModel> SaveLeadAsync(LeadModel model)
        {
            await _collection.ReplaceOneAsync(model.Id, model, true);
            return model;
        }

        public async  Task<bool> DeleteAllAsync(string customerId)
        {
            return await _collection.DeleteManyAsync(l=> l.CustomerId == customerId);
        }
    }
}
