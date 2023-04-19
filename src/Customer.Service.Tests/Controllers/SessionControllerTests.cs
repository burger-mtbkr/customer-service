namespace Customer.Service.UnitTests.Controllers
{
    public class SessionControllerTests: IDisposable
    {
        private readonly Mock<ISessionService> _mockSessionService;

        public SessionControllerTests()
        {
            _mockSessionService = new Mock<ISessionService>();
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
