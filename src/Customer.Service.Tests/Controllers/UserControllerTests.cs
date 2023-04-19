namespace Customer.Service.UnitTests.Controllers
{
    public class UserControllerTests: IDisposable
    {
        private readonly Mock<IUserService> _mockUserService;

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

        private readonly IEnumerable<UserModel> _users = new List<UserModel>
        {
            new UserModel
            {
                Id = _mockUserId,
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDateUtc = DateTime.UtcNow,
            },
            new UserModel
            {
                Id = "81304C8F-7E5C-49E3-7756-2EA81137A18P",
                Email = "other.user@mock.com",
                FirstName = "Other",
                LastName = "UserTestSurname",
                Password = "foo_bar_fred",
                Salt = "ate_some_bread",
                CreatedDateUtc = DateTime.UtcNow,
            },
        };

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public void Get_by_id_returns_valid_ok_response()
        {
            _mockUserService.Setup(s => s.GetUser(It.IsAny<string>())).Returns(_mockUser);

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void Get_by_id_returns_correct_object_type()
        {
            _mockUserService.Setup(s => s.GetUser(It.IsAny<string>())).Returns(_users.First());

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<UserModel>(result.Value);
        }

        [Fact]
        public void Get_by_id_returns_user_object()
        {
            _mockUserService.Setup(s => s.GetUser(It.IsAny<string>())).Returns(_users.First());

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var item = (UserModel)result.Value!;
            Assert.NotNull(item);
        }

        [Fact]
        public async Task Patch_returns_no_content_when_updating_user_password()
        {
            _mockUserService.Setup(s => s.ChangePasswordAsync(It.IsAny<string>(), It.IsAny<PasswordChangeRequest>())).ReturnsAsync(true);
            var controller = new UserController(_mockUserService.Object);
            var response = await controller.Patch(It.IsAny<string>(), It.IsAny<PasswordChangeRequest>());
            Assert.IsType<NoContentResult>(response);
        }

        public void Dispose()
        {
            _mockUserService.Reset();
        }
    }
}
