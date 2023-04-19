namespace Customer.Service.Tests.Controllers
{
    public class LeadsControllerTests: IDisposable
    {
        private Mock<ILeadsService> _mockLeadsService;
        private readonly IEnumerable<LeadModel> _mockLeads = new List<LeadModel>
        {
            new LeadModel
            {
                Id = "71204C8F-7E5C-49E3-9932-2EA81134E30E",
                CustomerId = "abcdef",
                Name = "Lead Name",
                Source = "Lead Source",
                Status = Enums.LeadStatus.NEW,
                CreatedDateUtc = DateTime.UtcNow,
            },
            new LeadModel
            {
                Id = "81304C8F-7E5C-49E3-7756-2EA81137A18P",
                CustomerId = "11111",
                Name = "Lead two Name",
                Source = "Lead two Source",
                Status = Enums.LeadStatus.CLOSED_LOST,
                CreatedDateUtc = DateTime.UtcNow,
            },
        };

        LeadModel _mockSingleLead = new()
        {
            Id = "12345",
            CustomerId = "11111",
            Name = "Single Lead",
            Source = "Single Lead Source",
            Status = Enums.LeadStatus.CLOSED_WON,
            CreatedDateUtc = DateTime.UtcNow,
        };

        public LeadsControllerTests()
        {
            _mockLeadsService = new Mock<ILeadsService>();
        }

        [Fact]
        public void Get_returns_valid_ok_response()
        {
            _mockLeadsService.Setup(s => s.GetLeads("11111")).Returns(_mockLeads);

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeads("11111");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void GetLeads_returns_correct_object_type()
        {
            var customerId = "11111";
            _mockLeadsService.Setup(s => s.GetLeads(customerId)).Returns(_mockLeads.Where(l => l.CustomerId == customerId));

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeads(customerId);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<LeadModel>>(result.Value);
        }

        [Fact]
        public void GetLeads_returns_all_stored_customers()
        {
            var customerId = "11111";
            _mockLeadsService.Setup(s => s.GetLeads(customerId)).Returns(_mockLeads.Where(l => l.CustomerId == customerId));

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeads(customerId);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var items = (IEnumerable<LeadModel>)result.Value!;
            Assert.Single(items);
        }

        [Fact]
        public void GetLeadById_returns_valid_ok_response()
        {
            _mockLeadsService.Setup(s => s.GetLeadById(It.IsAny<string>())).Returns(_mockLeads.First());

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeadById(_mockLeads.First().Id);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void GetLeadById_returns_correct_object_type()
        {
            _mockLeadsService.Setup(s => s.GetLeadById(It.IsAny<string>())).Returns(_mockLeads.First());

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeadById(_mockLeads.First().Id);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<LeadModel>(result.Value);
        }

        [Fact]
        public void GetLeadById_returns_customer_object()
        {
            _mockLeadsService.Setup(s => s.GetLeadById(It.IsAny<string>())).Returns(_mockLeads.First());

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = controller.GetLeadById(_mockLeads.First().Id);

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var item = (LeadModel)result.Value!;
            Assert.NotNull(item);
        }

        [Fact]
        public async Task Post_returns_a_new_valid_customer_with_valid_request()
        {
            _mockLeadsService.Setup(s => s.CreateLeadAsync(_mockSingleLead)).ReturnsAsync(_mockSingleLead);

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = await controller.Post(_mockSingleLead);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);

            var result = (CreatedResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("/api/leads", result.Location);
            Assert.Equal(_mockSingleLead, result.Value);
        }


        [Fact]
        public async Task Put_returns_no_content_when_updating_a_customer_by_id()
        {
            _mockLeadsService.Setup(s => s.UpdateLeadAsync(_mockSingleLead.Id, _mockSingleLead)).ReturnsAsync(true);

            var controller = new LeadsController(_mockLeadsService.Object);
            var response = await controller.Put(_mockSingleLead.Id, _mockSingleLead);
            Assert.IsType<NoContentResult>(response);
        }

        public void Dispose()
        {
            _mockLeadsService.Reset();
        }
    }
}
