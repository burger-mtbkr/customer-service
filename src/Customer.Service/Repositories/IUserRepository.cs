using Customer.Service.Models;

namespace Customer.Service.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="id"></param>        
        /// <returns></returns>
        UserModel? GetUser(string id);

        /// <summary>
        /// Get User By Email
        /// </summary>
        /// <param name="email"></param>        
        /// <returns></returns>
        UserModel? GetUserByEmail(string email);

        /// <summary>
        /// Stores the Session in the DB
        /// </summary>
        /// <returns></returns>
        Task<UserModel> CreateUserAsync(UserModel model);

        /// <summary>
        /// EDIT an existing user - TODO
        /// </summary>
        /// <param name="model"></param>        
        /// <returns></returns>
        Task<UserModel> EditUserAsync(UserModel model);

        /// <summary>
        /// Validate that a user email is available for use
        /// </summary>
        /// <param name="email"></param>        
        /// <returns></returns>
        bool CheckEmailAvailability(string email);
    }
}
