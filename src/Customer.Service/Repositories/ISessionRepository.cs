using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface ISessionRepository
    {
        /// <summary>
        /// Get all Sessions
        /// </summary>    
        /// <returns></returns>
        IEnumerable<Session> GetAll();

        /// <summary>
        /// Stores the Session in the DB
        /// </summary>
        /// <returns></returns>
        Task<Session> CreateSession(Session session);

        /// <summary>
        /// Deletes a session object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteSession(string id);

        /// <summary>
        /// Delete session by token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<bool> DeleteCurrentSession(string accessToken);

        /// <summary>
        /// Delete all sessions for a user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<bool> DeleteAllSessionForUser(string userId);

        /// <summary>
        /// Get  session by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Session? GetSession(string? id);
    }
}
