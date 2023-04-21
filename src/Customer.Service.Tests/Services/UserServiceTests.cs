namespace Customer.Service.Tests.Services
{
    public class UserServiceTests: IDisposable
    {
        private readonly Mock<IPasswordHash> _mockPasswordHash;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConfiguration> _mockConfiguration;

        private const string _mockUserId = "5AC89FEA-24C7-4834-A063-8846D23BF92F";
        private readonly UserModel _mockUser = new()
        {
            Id = _mockUserId,
            Email = "test.user@mock.com",
            FirstName = "Test",
            LastName = "TestSurname",
            Password = "password",
            Salt = "passwordSaltString",
            CreatedDateUtc = DateTime.UtcNow,
        };

        public UserServiceTests()
        {
            _mockPasswordHash = new Mock<IPasswordHash>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockUserRepository = new Mock<IUserRepository>();
        }

        [Fact]
        public void CheckEmailAvailability_returns_true_with_valid_email()
        {
            _mockUserRepository.Setup(c => c.CheckEmailAvailability(It.IsAny<string>())).Returns(true);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = service.CheckEmailAvailability("some@email.com");
            Assert.True(result);
            _mockUserRepository.Verify(r => r.CheckEmailAvailability("some@email.com"), Times.Once);
        }

        [Fact]
        public void CheckEmailAvailability_returns_false_with_with_unmatched_email()
        {
            _mockUserRepository.Setup(c => c.CheckEmailAvailability(It.IsAny<string>())).Returns(false);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = service.CheckEmailAvailability("some@email.com");
            Assert.False(result);
            _mockUserRepository.Verify(r => r.CheckEmailAvailability(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateUser_throws_ArgumentNullException_when_model_is_null()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.CreateUserAsync(null));

            Assert.NotNull(result);
            Assert.Equal("Value cannot be null. (Parameter 'model')", result.Message);
        }

        [Fact]
        public async Task CreateUser_throws_ArgumentException_when_email_is_not_provided()
        {
            var signupModel = new SignupRequest
            {
                Email = "",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
            };

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateUserAsync(signupModel));

            Assert.NotNull(result);
            Assert.Equal("Email is required", result.Message);
        }

        [Fact]
        public async Task CreateUser_throws_ArgumentException_when_name_is_not_provided()
        {
            var signupModel = new SignupRequest
            {
                Email = "test.user@mock.com",
                LastName = "TestSurname",
                Password = "password",
            };

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateUserAsync(signupModel));

            Assert.NotNull(result);
            Assert.Equal("FirstName is required", result.Message);
        }

        [Fact]
        public async Task CreateUser_throws_ArgumentException_when_lastname_is_not_provided()
        {
            var signupModel = new SignupRequest
            {
                Email = "test.user@mock.com",
                FirstName = "Test",
                Password = "password",
            };

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateUserAsync(signupModel));

            Assert.NotNull(result);
            Assert.Equal("LastName is required", result.Message);
        }

        [Fact]
        public async Task CreateUser_throws_ArgumentException_when_password_is_not_provided()
        {
            var signupModel = new SignupRequest
            {
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "",
            };

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateUserAsync(signupModel));

            Assert.NotNull(result);
            Assert.Equal("Password is required", result.Message);
        }

        [Fact]
        public async Task CreateUser_throws_EmailAlreadyInUseException_when_email_is_in_use()
        {
            var signupModel = new SignupRequest
            {
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
            };

            _mockUserRepository.Setup(c => c.CheckEmailAvailability(It.IsAny<string>())).Returns(false);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<EmailAlreadyInUseException>(async () => await service.CreateUserAsync(signupModel));
            Assert.NotNull(result);
            Assert.Equal("Email is already in use.", result.Message);
        }

        [Fact]
        public async Task CreateUser_returns_a_valid_user()
        {
            var signupModel = new SignupRequest
            {
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
            };

            _mockUserRepository.Setup(c => c.CheckEmailAvailability(It.IsAny<string>())).Returns(true);
            _mockUserRepository.Setup(c => c.CreateUserAsync(It.IsAny<UserModel>())).ReturnsAsync(_mockUser);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await service.CreateUserAsync(signupModel);

            Assert.NotNull(result);
            Assert.Equal(_mockUser.Id, result.Id);
            Assert.Equal(signupModel.Email, result.Email);
            Assert.Equal(signupModel.FirstName, result.FirstName);
            Assert.Equal(signupModel.LastName, result.LastName);
            _mockUserRepository.Verify(r => r.CreateUserAsync(It.IsAny<UserModel>()), Times.Once);
        }

        [Fact]
        public void GetUser_returns_user_matching_id()
        {
            _mockUserRepository.Setup(c => c.GetUser(_mockUserId)).Returns(_mockUser);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);

            var result = service.GetUser(_mockUserId);

            Assert.NotNull(result);
            Assert.Equal(_mockUser.Id, result.Id);
            Assert.Equal(_mockUser.Email, result.Email);
            Assert.Equal(_mockUser.FirstName, result.FirstName);
            Assert.Equal(_mockUser.LastName, result.LastName);
            Assert.Equal(_mockUser.Password, result.Password);
            Assert.Equal(_mockUser.Salt, result.Salt);
            Assert.Equal(_mockUser.CreatedDateUtc, result.CreatedDateUtc);

            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Once);
        }

        [Fact]
        public void GetUser_throws_ArgumentNullException_when_id_is_null()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<ArgumentNullException>(() => service.GetUser(null));
        }

        [Fact]
        public void GetUser_throws_ArgumentNullException_when_id_is_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<ArgumentNullException>(() => service.GetUser(string.Empty));
        }

        [Fact]
        public void GetUser_throws_UserNotFoundException_when_id_does_not_match_any_users()
        {
            var id = "5648787786";
            _mockUserRepository.Setup(c => c.GetUser(It.IsAny<string>())).Returns<UserModel>(null);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<UserNotFoundException>(() => service.GetUser(id));
            _mockUserRepository.Verify(r => r.GetUser(id), Times.Once);
        }

        [Fact]
        public void GetUserByEmail_returns_user_matching_email()
        {
            _mockUserRepository.Setup(c => c.GetUserByEmail(_mockUser.Email)).Returns(_mockUser);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = service.GetUserByEmail(_mockUser.Email);

            Assert.NotNull(result);
            Assert.Equal(_mockUser.Id, result.Id);
            Assert.Equal(_mockUser.Email, result.Email);
            Assert.Equal(_mockUser.FirstName, result.FirstName);
            Assert.Equal(_mockUser.LastName, result.LastName);
            Assert.Equal(_mockUser.Password, result.Password);
            Assert.Equal(_mockUser.Salt, result.Salt);
            Assert.Equal(_mockUser.CreatedDateUtc, result.CreatedDateUtc);

            _mockUserRepository.Verify(r => r.GetUserByEmail(_mockUser.Email), Times.Once);
        }

        [Fact]
        public void GetUserByEmail_throws_ArgumentNullException_when_email_is_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<ArgumentNullException>(() => service.GetUserByEmail(string.Empty));
        }

        [Fact]
        public void GetUserByEmail_throws_UserNotFoundException_when_email_has_no_matching_user()
        {
            var wrongEmail = "abs@xyz.com";
            _mockUserRepository.Setup(c => c.GetUserByEmail(It.IsAny<string>())).Returns<UserModel>(null);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<UserNotFoundException>(() => service.GetUserByEmail(wrongEmail));
            _mockUserRepository.Verify(r => r.GetUserByEmail(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ValidateUserPassword_returns_true_if_password_is_valid()
        {
            _mockUserRepository.Setup(c => c.GetUser(_mockUserId)).Returns(_mockUser);
            _mockPasswordHash.Setup(p => p.CreatePasswordSalt(It.IsAny<string>(), It.IsAny<string>())).Returns("hashed_password");
            _mockPasswordHash.Setup(p => p.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);

            var result = service.ValidateUserPassword(_mockUserId, "password");

            Assert.True(result);
            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Once);
            _mockPasswordHash.Verify(r => r.CreateSaltedPassword(It.IsAny<string>(), _mockUser.Salt), Times.Once);
            _mockPasswordHash.Verify(r => r.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ValidateUserPassword_throws_ArgumentNullException_when_userId_is_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<ArgumentNullException>(() => service.ValidateUserPassword(string.Empty, "password"));
        }

        [Fact]
        public void ValidateUserPassword_throws_ArgumentNullException_when_presentedPassword_is_is_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<ArgumentNullException>(() => service.ValidateUserPassword("12345", string.Empty));
        }

        [Fact]
        public void ValidateUserPassword_throws_UserNotFoundException_when_no_matching_user_is_found()
        {
            _mockUserRepository.Setup(c => c.GetUser(It.IsAny<string>())).Returns<UserModel>(null);
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            Assert.Throws<UserNotFoundException>(() => service.ValidateUserPassword("8888", "password"));
            _mockUserRepository.Verify(r => r.GetUser(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void ValidateUserPassword_returns_false_when_passwords_dont_match()
        {
            _mockUserRepository.Setup(c => c.GetUser(It.IsAny<string>())).Returns(_mockUser);
            _mockPasswordHash.Setup(p => p.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = service.ValidateUserPassword(_mockUserId, "password123");

            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Once);
            _mockPasswordHash.Verify(r => r.CreateSaltedPassword(It.IsAny<string>(), _mockUser.Salt), Times.Once);
            _mockPasswordHash.Verify(r => r.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_throws_ArgumentNullException_when_userId_is_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ChangePasswordAsync(string.Empty, It.IsAny<PasswordChangeRequest>()));
            Assert.Equal("Value cannot be null. (Parameter 'userId')", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_throws_ArgumentNullException_when_model_is_null()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.ChangePasswordAsync("password", null));
            Assert.Equal("Value cannot be null. (Parameter 'model')", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsyncAsync_throws_ArgumentNullException_when_model_new_password_is_null_or_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.ChangePasswordAsync(_mockUserId, new PasswordChangeRequest
            {
                NewPassword = "",
                OldPassword = "password",
            }));

            Assert.Equal("NewPassword", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsyncAsync_throws_ArgumentNullException_when_model_old_password_is_null_or_empty()
        {
            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(() => service.ChangePasswordAsync(_mockUserId, new PasswordChangeRequest
            {
                NewPassword = "paassword",
                OldPassword = "",
            }));

            Assert.Equal("OldPassword", result.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_throws_UserNotFoundException_when_no_user_is_found()
        {
            _mockUserRepository.Setup(u => u.GetUser(_mockUserId)).Returns<UserModel>(null);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);
            var result = await Assert.ThrowsAsync<UserNotFoundException>(() => service.ChangePasswordAsync(_mockUserId, new PasswordChangeRequest
            {
                NewPassword = "paassword",
                OldPassword = "abcdef",
            }));

            Assert.Equal($"User not found for id {_mockUserId}", result.Message);
            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_throws_InvalidPasswordException_if_old_password_is_invalid()
        {
            var request = new PasswordChangeRequest
            {
                NewPassword = "paassword",
                OldPassword = "abcdef",
            };

            _mockUserRepository.Setup(c => c.GetUser(_mockUserId)).Returns(_mockUser);
            _mockPasswordHash.Setup(p => p.CreatePasswordSalt(It.IsAny<string>(), It.IsAny<string>())).Returns("hashed_password");
            _mockPasswordHash.Setup(p => p.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);

            var result = await Assert.ThrowsAsync<InvalidPasswordException>(() => service.ChangePasswordAsync(_mockUserId, request));

            Assert.Equal("The OldPassword is not correct", result.Message);
            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Once);
            _mockPasswordHash.Verify(r => r.CreateSaltedPassword(It.IsAny<string>(), _mockUser.Salt), Times.Once);
            _mockPasswordHash.Verify(r => r.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ChangePasswordAsync_returns_true_if_old_password_is_valid()
        {
            _mockUserRepository.Setup(c => c.GetUser(_mockUserId)).Returns(_mockUser);
            _mockPasswordHash.Setup(p => p.CreatePasswordSalt(It.IsAny<string>(), It.IsAny<string>())).Returns("hashed_password");
            _mockPasswordHash.Setup(p => p.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _mockUserRepository.Setup(u => u.EditUserAsync(_mockUser)).ReturnsAsync(_mockUser);

            var service = new UserService(_mockConfiguration.Object, _mockUserRepository.Object, _mockPasswordHash.Object);

            var result = await service.ChangePasswordAsync(_mockUserId, new PasswordChangeRequest
            {
                NewPassword = "paassword",
                OldPassword = "abcdef",
            });

            Assert.True(result);

            _mockUserRepository.Verify(r => r.GetUser(_mockUserId), Times.Exactly(2));
            _mockPasswordHash.Verify(r => r.CreateSaltedPassword(It.IsAny<string>(), _mockUser.Salt), Times.Exactly(2));
            _mockPasswordHash.Verify(r => r.CompareByteArrayStrings(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        public void Dispose()
        {
            _mockPasswordHash.Reset();
            _mockUserRepository.Reset();
            _mockConfiguration.Reset();
        }
    }
}
