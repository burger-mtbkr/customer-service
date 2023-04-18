using Customer.Service.Controllers;
using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace Customer.Service.UnitTests.Controllers
{
    public class SignupControllerTests: IDisposable
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<ISessionService> _mockSessionService;

        public SignupControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockSessionService = new Mock<ISessionService>();
        }


        [Fact]
        public async Task Post_returns_a_badrequest_with_inull_model_session_model_request()
        {
            var controller = new SignupController(_mockUserService.Object, _mockSessionService.Object);
            var response = await controller.Post(null);

            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);

            var result = (BadRequestObjectResult)response.Result;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);

            Assert.Equal("Email or password has not been provoded", result.Value);
        }

        [Fact]
        public async Task Post_returns_a_badrequest_with_invalid_session_request()
        {
            var mockSignupRequest = new SignupRequest();
            var controller = new SignupController(_mockUserService.Object, _mockSessionService.Object);
            var response = await controller.Post(mockSignupRequest);

            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);

            var result = (BadRequestObjectResult)response.Result;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);

            Assert.Equal("Email or password has not been provoded", result.Value);
        }

        [Fact]
        public async Task Post_throws_exception_when_exception_occurs_in_user_service()
        {
            var mockSignupRequest = new SignupRequest
            {
                Email = "test@email.com",
                FirstName = "Test",
                LastName = "Test",
                Password = "Test",
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<SignupRequest>())).Throws<Exception>();
            var controller = new SignupController(_mockUserService.Object, _mockSessionService.Object);
            await Assert.ThrowsAsync<Exception>(async () => await controller.Post(mockSignupRequest));
        }

        [Fact]
        public async Task Post_throws_exception_when_exception_occurs_in_session_service()
        {
            var mockSignupRequest = new SignupRequest
            {
                Email = "test@email.com",
                FirstName = "Test",
                LastName = "Test",
                Password = "Test",
            };

            var mockUser = new UserModel
            {
                Id = "654856556",
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDate = DateTime.UtcNow,
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<SignupRequest>())).ReturnsAsync(mockUser);
            _mockSessionService.Setup(s => s.CreateSession(It.IsAny<string>())).Throws<Exception>();

            var controller = new SignupController(_mockUserService.Object, _mockSessionService.Object);

            await Assert.ThrowsAsync<Exception>(async () => await controller.Post(mockSignupRequest));
        }    

        [Fact]
        public async Task Post_returns_a_new_valid_session_with_valid_signup_request()
        {
            var mockSignupRequest = new SignupRequest
            {
                Email = "test@email.com",
                FirstName = "Test",
                LastName = "Test",
                Password = "Test",
            };

            var mockUser = new UserModel
            {
                Id = "654856556",
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDate = DateTime.UtcNow,
            };

            _mockUserService.Setup(s => s.CreateUserAsync(It.IsAny<SignupRequest>())).ReturnsAsync(mockUser);
            _mockSessionService.Setup(s => s.CreateSession(It.IsAny<string>())).ReturnsAsync("some_generated_jwt_session");

            var controller = new SignupController(_mockUserService.Object, _mockSessionService.Object);

            var response = await controller.Post(mockSignupRequest);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);

            var result = (CreatedResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("/api/signup", result.Location);
            Assert.Equal("some_generated_jwt_session", result.Value);
        }

        public void Dispose()
        {
            _mockUserService.Reset();
            _mockSessionService.Reset();
        }
    }
}
