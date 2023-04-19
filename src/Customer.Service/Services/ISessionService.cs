using Customer.Service.Models;
namespace Customer.Service.Services
{
    public interface ISessionService
    {
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
        /// Delete the current session
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteCurrentSession();

        /// <summary>
        /// Validates if a token is active.
        /// </summary>
        /// <returns></returns>
        bool IsTokenActive();
    }
}