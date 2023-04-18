namespace Customer.Service.UnitTests.Controllers
{
    public class LoginControllerTests: IDisposable
    {
        private readonly Mock<ILoginService> _mockLoginService;

        public LoginControllerTests()
        {
            _mockLoginService = new Mock<ILoginService>();
        }

        [Fact]
        public async Task Post_returns_a_badrequest_with_null_model_login_request_model()
        {
            var controller = new LoginController(_mockLoginService.Object);
            var response = await controller.Post(null);

            Assert.NotNull(response);
            Assert.IsType<BadRequestObjectResult>(response.Result);

            var result = (BadRequestObjectResult)response.Result;

            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);

            Assert.Equal("Email or password has not been provoded", result.Value);
        }

        [Fact]
        public async Task Post_returns_a_badrequest_with_invalid_login_request()
        {
            var mockLoginRequest = new LoginRequest();
            var controller = new LoginController(_mockLoginService.Object);
            var response = await controller.Post(mockLoginRequest);

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
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = "Test",
            };

            _mockLoginService.Setup(s => s.Login(It.IsAny<LoginRequest>())).Throws<Exception>();
            var controller = new LoginController(_mockLoginService.Object);
            await Assert.ThrowsAsync<Exception>(async () => await controller.Post(mockLoginRequest));
        }

        [Fact]
        public async Task Post_returns_a_new_valid_session_with_valid_signup_request()
        {
            var mockLoginRequest = new LoginRequest
            {
                Email = "test@email.com",
                Password = "Test",
            };

            _mockLoginService.Setup(s => s.Login(It.IsAny<LoginRequest>())).ReturnsAsync("some_generated_jwt_session");

            var controller = new LoginController(_mockLoginService.Object);

            var response = await controller.Post(mockLoginRequest);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);

            var result = (CreatedResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("/api/login", result.Location);
            Assert.Equal("some_generated_jwt_session", result.Value);
        }

        [Fact]
        public async Task Post_returns_no_content_when_logging_out()
        {
            _mockLoginService.Setup(s => s.Logout()).ReturnsAsync(true);
            var controller = new LoginController(_mockLoginService.Object);
            var response = await controller.Delete();
            Assert.IsType<NoContentResult>(response);
        }

        public void Dispose()
        {
            _mockLoginService.Reset();
        }
    }
}
