namespace Customer.Service.UnitTests.Controllers
{
    public class HealthCheckControllerTests
    {
        public HealthCheckControllerTests() { }

        [Fact]
        public void Get_returns_emptry_ok_result()
        {
            var controller = new HealthCheckController();
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkResult>(response);
            Assert.Equal(200, ((OkResult)response).StatusCode);
        }
    }
}
