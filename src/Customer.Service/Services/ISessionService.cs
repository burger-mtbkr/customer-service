using Customer.Service.Models;
namespace Customer.Service.Services
{
    public interface ISessionService
    {
        /// <summary>
        /// Get all Sessions
        /// </summary>
        /// <returns></returns>
        IEnumerable<Session> GetAll();

        /// <summary>
        /// Get session By token
        /// </summary>
        /// <returns></returns>
        Session? GetSession(string id);

        /// <summary>
        /// Stores the Session in the DB
        /// </summary>
        /// <returns></returns>
        Task<string?> CreateSession(string userId);

        /// <summary>
        /// Deletes a session object
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteSession(string id);

        /// <summary>
        /// Delete the current session
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteCurrentSession();

        /// <summary>
        /// Delete all sessions for a user
        /// </summary>        
        /// <returns></returns>
        Task<bool> DeleteAllSessionForCurrentUser();

        /// <summary>
        /// Validates if a token is active.
        /// </summary>
        /// <returns></returns>
        bool IsTokenActive();
    }
}