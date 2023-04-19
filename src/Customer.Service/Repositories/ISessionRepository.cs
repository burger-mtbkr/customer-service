using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface ISessionRepository
    {
        /// <summary>
        /// Stores the Session in the DB
        /// </summary>
        /// <returns></returns>
        Task<Session> CreateSession(Session session);

        /// <summary>
        /// Delete session by token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<bool> DeleteCurrentSession(string accessToken);

        /// <summary>
        /// Get  session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Session? GetSession(string? id);
    }
}
