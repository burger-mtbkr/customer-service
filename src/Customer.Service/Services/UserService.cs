using Customer.Service.Exceptions;
using Customer.Service.Infrastructure.Auth;
using Customer.Service.Models;
using Customer.Service.Repositories;

namespace Customer.Service.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly IPasswordHash _passwordHash;

        public UserService(IConfiguration configuration, IUserRepository userRepository, IPasswordHash passwordHash)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _passwordHash = passwordHash;
        }

        public bool CheckEmailAvailability(string email)
        {
            return _userRepository.CheckEmailAvailability(email);
        }

        public async Task<UserModel> CreateUserAsync(SignupRequest model)
        {
            if(model == null) throw new ArgumentNullException(nameof(model));
            if(string.IsNullOrEmpty(model.Email)) throw new ArgumentException($"{nameof(model.Email)} is required");
            if(string.IsNullOrEmpty(model.FirstName)) throw new ArgumentException($"{nameof(model.FirstName)} is required");
            if(string.IsNullOrEmpty(model.LastName)) throw new ArgumentException($"{nameof(model.LastName)} is required");
            if(string.IsNullOrEmpty(model.Password)) throw new ArgumentException($"{nameof(model.Password)} is required");

            var emailAvailable = CheckEmailAvailability(model.Email);
            if(!emailAvailable)
            {
                throw new EmailAlreadyInUseException("Email is already in use.");
            }

            //Encrypt password
            var platformHash = _configuration["Platform:Salt_Guid"];
            var salt = _passwordHash.CreatePasswordSalt(platformHash!, model.Email);
            var saltedPassword = _passwordHash.CreateSaltedPassword(model.Password, salt);

            UserModel user = new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = saltedPassword,
                Salt = salt,
                CreatedDateUtc = DateTime.UtcNow,
            };

            return await _userRepository.CreateUserAsync(user);
        }

        public UserModel GetUser(string id)
        {
            if(string.IsNullOrEmpty(id)) throw new ArgumentNullException(nameof(id));
            var user = _userRepository.GetUser(id);
            if(user == null) throw new UserNotFoundException($"User not found for id {id}");
            return user;
        }

        public UserModel GetUserByEmail(string email)
        {
            if(string.IsNullOrEmpty(email)) throw new ArgumentNullException(nameof(email));
            var user = _userRepository.GetUserByEmail(email);
            if(user == null) throw new UserNotFoundException($"User not found for email {email}");
            return user;
        }

        public bool ValidateUserPassword(string userId, string presentedPassword)
        {
            if(string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if(string.IsNullOrEmpty(presentedPassword)) throw new ArgumentNullException(nameof(presentedPassword));
            var user = GetUser(userId);

            // Take the txt password we get sent and apply the salt saved against the user then compare it with the password stored in the database
            var hashedPresentedPassword = _passwordHash.CreateSaltedPassword(presentedPassword, user!.Salt);
            return _passwordHash.CompareByteArrayStrings(user.Password, hashedPresentedPassword);
        }

        public async Task<bool> ChangePasswordAsync(string userId, PasswordChangeRequest model)
        {
            if(string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));
            if(model == null) throw new ArgumentNullException(nameof(model));
            if(string.IsNullOrEmpty(model.NewPassword)) throw new ArgumentException(nameof(model.NewPassword));
            if(string.IsNullOrEmpty(model.OldPassword)) throw new ArgumentException(nameof(model.OldPassword));

            var oldPasswordValid = ValidateUserPassword(userId, model.OldPassword);
            if(!oldPasswordValid) throw new InvalidPasswordException($"The {nameof(model.OldPassword)} is not correct");

            var user = GetUser(userId);
            user.Password = _passwordHash.CreateSaltedPassword(model.NewPassword, user!.Salt);
            var result = await _userRepository.EditUserAsync(user);
            return result != null;
        }  
    }
}
