using Customer.Service.Models;
using JsonFlatFileDataStore;

namespace Customer.Service.Repositories
{
    public class SessionRepository: ISessionRepository
    {
        private readonly ILogger<SessionRepository> _logger;
        private readonly IDocumentCollection<Session> _colletion;
        public SessionRepository(IDocumentCollection<Session> colection, ILogger<SessionRepository> logger)
        {
            _logger = logger;
            _colletion = colection;
        }

        public async Task<Session> CreateSession(Session session)
        {
            await _colletion.InsertOneAsync(session);
            return session;
        }    

        public Session? GetSession(string? id)
        {
            return _colletion.AsQueryable().FirstOrDefault(t => t.Id == id);
        }
     
        public async Task<bool> DeleteCurrentSession(string accessToken)
        {
            return await _colletion.DeleteOneAsync(s => s.Token == accessToken);
        }  
    }
}
