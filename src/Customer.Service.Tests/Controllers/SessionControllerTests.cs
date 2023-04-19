namespace Customer.Service.UnitTests.Controllers
{
    public class SessionControllerTests: IDisposable
    {
        private readonly Mock<ISessionService> _mockSessionService;

        private readonly IEnumerable<Session> _sessions = new List<Session>
        {
            new Session
            {
                Id = Guid.NewGuid().ToString(),
                CreatedDateUtc = DateTime.UtcNow,
                Token = Guid.NewGuid().ToString(),
                UserEmail = "some.test@user.com",
                UserId = Guid.NewGuid().ToString(),
                Expiry = DateTime.UtcNow.AddHours(720),
            },
            new Session {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                CreatedDateUtc = DateTime.UtcNow,
                Token = "boohoo123456778",
                UserEmail = "some.test@user.com",
                UserId = "B15A0836-BCBF-49DC-83E7-0F9D962C2A79",
                Expiry = DateTime.UtcNow.AddHours(720),
            }
        };

        public SessionControllerTests()
        {
            _mockSessionService = new Mock<ISessionService>();
        }

        [Fact]
        public void GetAll_returns_valid_ok_response()
        {
            _mockSessionService.Setup(s => s.GetAll()).Returns(_sessions);

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void GetAll_returns_correct_object_type()
        {
            _mockSessionService.Setup(s => s.GetAll()).Returns(_sessions);

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<Session>>(result.Value);
        }

        [Fact]
        public void GetAll_returns_all_stored_Sessions()
        {
            _mockSessionService.Setup(s => s.GetAll()).Returns(_sessions);

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var items = (IEnumerable<Session>)result.Value!;
            Assert.Equal(2, items.Count());
        }

        [Fact]
        public void GetSession_returns_valid_ok_response()
        {
            _mockSessionService.Setup(s => s.GetSession(It.IsAny<string>())).Returns(_sessions.First());

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void GetSession_returns_correct_object_type()
        {
            _mockSessionService.Setup(s => s.GetSession(It.IsAny<string>())).Returns(_sessions.First());

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<Session>(result.Value);
        }

        [Fact]
        public void GetSession_returns_sessions()
        {
            _mockSessionService.Setup(s => s.GetSession(It.IsAny<string>())).Returns(_sessions.First());

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var item = (Session)result.Value!;
            Assert.NotNull(item);
        }

        [Fact]
        public async Task DeleteSession_returns_no_content_when_logging_out()
        {
            _mockSessionService.Setup(s => s.DeleteSession(It.IsAny<string>())).ReturnsAsync(true);
            var controller = new SessionController(_mockSessionService.Object);
            var response = await controller.Delete("id");
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task DeleteAllSessionForCurrentUser_returns_no_content_when_logging_out()
        {
            _mockSessionService.Setup(s => s.DeleteAllSessionForCurrentUser()).ReturnsAsync(true);
            var controller = new SessionController(_mockSessionService.Object);
            var response = await controller.Delete();
            Assert.IsType<NoContentResult>(response);
        }


        [Fact]
        public void IsTokenActive_returns_correct_true_Ok__with_true_when_active()
        {
            _mockSessionService.Setup(s => s.IsTokenActive()).Returns(true);

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.IsTokenActive();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void IsTokenActive_returns_correct_true_Ok__with_false_when_active()
        {
            _mockSessionService.Setup(s => s.IsTokenActive()).Returns(false);

            var controller = new SessionController(_mockSessionService.Object);
            var response = controller.IsTokenActive();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            Assert.Equal(200, result.StatusCode);
        }

        public void Dispose()
        {
            _mockSessionService.Reset();
        }
    }
}
