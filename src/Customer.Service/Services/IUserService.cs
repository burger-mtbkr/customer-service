using Customer.Service.Models;

namespace Customer.Service.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Get all users for application
        /// </summary>
        /// <returns></returns>
        IEnumerable<UserModel> GetAllUsers();

        /// <summary>
        /// Get User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserModel GetUser(string id);

        /// <summary>
        /// Get User By Email
        /// </summary>
        /// <param name="email"></param> 
        /// <returns></returns>
        UserModel GetUserByEmail(string email);

        /// <summary>
        /// Stores the Session in the DB
        /// </summary>
        /// <returns></returns>
        Task<UserModel> CreateUserAsync(SignupRequest model);

        /// <summary>
        /// EDIT an existing user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<UserModel> EditUserAsync(string id, UserModel model);

        /// <summary>
        ///  Updates a user's password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> ChangePasswordAsync(string id, PasswordChangeRequest model);

        /// <summary>
        /// Marks a user as deleted
        /// Marks any user sessions as expired
        /// Marks any Assigned Application as Deleted
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteUserAsync(string id);

        /// <summary>
        /// Validate that a user email is available for use
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool CheckEmailAvailability(string email);

        /// <summary>
        /// Validate User Password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="presentedPassword"></param>
        /// <returns></returns>
        bool ValidateUserPassword(string userId, string presentedPassword);
    }
}
