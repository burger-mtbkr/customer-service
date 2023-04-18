using Customer.Service.Controllers;
using Customer.Service.Models;
using Customer.Service.Services;
using Microsoft.AspNetCore.Mvc;

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
            CreatedDate = DateTime.UtcNow,
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
                CreatedDate = DateTime.UtcNow,
            },
            new UserModel
            {
                Id = "81304C8F-7E5C-49E3-7756-2EA81137A18P",
                Email = "other.user@mock.com",
                FirstName = "Other",
                LastName = "UserTestSurname",
                Password = "foo_bar_fred",
                Salt = "ate_some_bread",
                CreatedDate = DateTime.UtcNow,
            },
        };

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
        }

        [Fact]
        public void Get_returns_valid_ok_response()
        {
            _mockUserService.Setup(s => s.GetAllUsers()).Returns(_users);

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void Get_returns_correct_object_type()
        {
            _mockUserService.Setup(s => s.GetAllUsers()).Returns(_users);

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<UserResponse>>(result.Value);
        }

        [Fact]
        public void Get_returns_all_stored_users()
        {
            _mockUserService.Setup(s => s.GetAllUsers()).Returns(_users);

            var controller = new UserController(_mockUserService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var items = (IEnumerable<UserResponse>)result.Value!;
            Assert.Equal(2, items.Count());
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
            Assert.IsAssignableFrom<UserResponse>(result.Value);
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
            var item = (UserResponse)result.Value!;
            Assert.NotNull(item);
        }

        [Fact]
        public async Task Put_returns_no_content_when_updating_a_user_by_id()
        {
            _mockUserService.Setup(s => s.EditUserAsync(_mockUserId, _mockUser)).ReturnsAsync(_mockUser);
            var controller = new UserController(_mockUserService.Object);
            var response = await controller.Put(_mockUserId, _mockUser);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_returns_no_content_when_deleting_a_user_by_id()
        {
            _mockUserService.Setup(s => s.DeleteUserAsync(It.IsAny<string>())).ReturnsAsync(true);
            var controller = new UserController(_mockUserService.Object);
            var response = await controller.Delete("id");
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Patch_returns_no_content_when_deleting_a_user_by_id()
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
