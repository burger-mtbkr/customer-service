namespace Customer.Service.Tests.Services
{
    public class LoginServiceTests: IDisposable
    {
        private readonly Mock<ISessionService> _mockSessionService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ITokenHelper> _mockTokenHelper;

        public LoginServiceTests()
        {
            _mockSessionService = new Mock<ISessionService>();
            _mockUserService = new Mock<IUserService>();
            _mockTokenHelper = new Mock<ITokenHelper>();
        }

        [Fact]
        public async Task Login_throws_ArgumentException_when_email_is_not_provided()
        {
            var mockLoginRequest = new LoginRequest
            {
                Password = "password"
            };

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(() => loginService.Login(mockLoginRequest));

            Assert.NotNull(result);
            Assert.Equal("Email is required", result.Message);
        }

        [Fact]
        public async Task Login_throws_ArgumentException_when_password_is_not_provided()
        {
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = ""
            };

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(() => loginService.Login(mockLoginRequest));

            Assert.NotNull(result);
            Assert.Equal("Password is required", result.Message);
        }

        [Fact]
        public async Task Login_throws_ArgumentNullException_when_password_is_not_provided()
        {
            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => loginService.Login(null));

            Assert.NotNull(result);
            Assert.Equal("request", result.ParamName);
            Assert.Equal("Value cannot be null. (Parameter 'request')", result.Message);
        }

        [Fact]
        public async Task Login_returns_a_token_with_valid_request()
        {
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = "password"
            };

            var mockUser = new UserModel
            {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                Email = "test@email.com",
                FirstName = "Test",
                LastName = "Test",
                CreatedDateUtc = DateTime.UtcNow,
                Password = "password",
                Salt = "86876876"
            };

            _mockUserService.Setup(u => u.GetUserByEmail(mockUser.Email)).Returns(mockUser);
            _mockUserService.Setup(u => u.ValidateUserPassword(mockUser.Id, mockLoginRequest.Password)).Returns(true);

            _mockSessionService.Setup(s => s.CreateSession(It.IsAny<string>())).ReturnsAsync("some_jwt_session_token");

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);

            var token = await loginService.Login(mockLoginRequest);

            Assert.NotNull(token);
            Assert.Equal("some_jwt_session_token", token);

            _mockUserService.Verify(u => u.GetUserByEmail(mockUser.Email), Times.Once);
            _mockUserService.Verify(u => u.ValidateUserPassword(mockUser.Id, mockLoginRequest.Password), Times.Once);
            _mockSessionService.Verify(u => u.CreateSession(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Login_throws_UserNotFoundException_if_no_user_is_found_for_the_email()
        {
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = "password"
            };

            _mockUserService.Setup(u => u.GetUserByEmail("test@email.com")).Returns<UserModel>(null);
            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<UserNotFoundException>(() => loginService.Login(mockLoginRequest));

            Assert.NotNull(result);
            Assert.IsType<UserNotFoundException>(result);
            Assert.Equal("User not found for test@email.com", result.Message);
            _mockUserService.Verify(u => u.GetUserByEmail("test@email.com"), Times.Once);
        }

        [Fact]
        public async Task Login_throws_ArgumentException_if_no_user_is_found_for_the_email()
        {
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = "password"
            };

            var mockUser = new UserModel
            {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                Email = "test@email.com",
                FirstName = "Test",
                LastName = "Test",
                CreatedDateUtc = DateTime.UtcNow,
                Password = "password",
                Salt = "86876876"
            };

            _mockUserService.Setup(u => u.GetUserByEmail(mockUser.Email)).Returns(mockUser);
            _mockUserService.Setup(u => u.ValidateUserPassword(mockUser.Id, "wrong")).Returns(false);

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<InvalidPasswordException>(() => loginService.Login(mockLoginRequest));

            Assert.NotNull(result);
            Assert.IsType<InvalidPasswordException>(result);
            Assert.Equal("Password is not valid", result.Message);

            _mockUserService.Verify(u => u.GetUserByEmail(mockUser.Email), Times.Once);
            _mockUserService.Verify(u => u.ValidateUserPassword(mockUser.Id, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task Logout_reurns_true_when_token_deleted()
        {
            _mockSessionService.Setup(u => u.DeleteCurrentSession()).ReturnsAsync(true);

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await loginService.Logout();

            Assert.True(result);
            _mockSessionService.Verify(u => u.DeleteCurrentSession(), Times.Once);
        }

        [Fact]
        public async Task Logout_reurns_false_when_token_is_not_deleted()
        {
            _mockSessionService.Setup(u => u.DeleteCurrentSession()).ReturnsAsync(false);

            var loginService = new LoginService(_mockSessionService.Object, _mockUserService.Object);
            var result = await loginService.Logout();

            Assert.False(result);
            _mockSessionService.Verify(u => u.DeleteCurrentSession(), Times.Once);

        }

        public void Dispose()
        {
            _mockSessionService.Reset();
            _mockUserService.Reset();
            _mockTokenHelper.Reset();
        }
    }
}
