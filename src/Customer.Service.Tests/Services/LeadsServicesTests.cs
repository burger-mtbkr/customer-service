using Castle.Core.Resource;

namespace Customer.Service.Tests.Services
{
    public class LeadsServicesTests: IDisposable
    {
        private readonly Mock<ILeadRepository> _mockLeadRepository;
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

        public LeadsServicesTests()
        {
            _mockLeadRepository = new Mock<ILeadRepository>();
        }


        [Fact]
        public void GetLeads_calls_GetLeads_repository()
        {
            _mockLeadRepository.Setup(u => u.GetLeads(It.IsAny<string>())).Returns(_mockLeads);

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = service.GetLeads("some_id");

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _mockLeadRepository.Verify(r => r.GetLeads(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetLeads_returns_empty_list_when_none_are_found()
        {
            _mockLeadRepository.Setup(u => u.GetLeads(It.IsAny<string>())).Returns(new List<LeadModel>());

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = service.GetLeads("1");

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockLeadRepository.Verify(r => r.GetLeads(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void GetLeadById_returns_customer_matching_id()
        {
            _mockLeadRepository.Setup(c => c.GetLeadById(_mockSingleLead.Id)).Returns(_mockSingleLead);

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = service.GetLeadById(_mockSingleLead.Id);

            Assert.NotNull(result);
            Assert.Equal(_mockSingleLead, result);

            _mockLeadRepository.Verify(r => r.GetLeadById(_mockSingleLead.Id), Times.Once);
        }

        [Fact]
        public void GetLeadById_throws_ArgumentNullException_when_id_is_null()
        {
            var service = new LeadsService(_mockLeadRepository.Object);
            var result = Assert.Throws<ArgumentNullException>(() => service.GetLeadById(null));
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        [Fact]
        public void GetLeadById_throws_ArgumentNullException_when_id_is_empty()
        {
            var service = new LeadsService(_mockLeadRepository.Object);
            var result = Assert.Throws<ArgumentNullException>(() => service.GetLeadById(string.Empty));
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        [Fact]
        public void GetLeadById_throws_LeadNotFoundException_when_id_does_not_match_any_customers()
        {
            var id = "5648787786";
            _mockLeadRepository.Setup(c => c.GetLeadById(It.IsAny<string>())).Returns<LeadModel>(null);
            var service = new LeadsService(_mockLeadRepository.Object);
            var result = Assert.Throws<LeadNotFoundException>(() => service.GetLeadById(id));
            _mockLeadRepository.Verify(r => r.GetLeadById(id), Times.Once);
            Assert.Equal($"Lead not found for id {id}", result.Message);
        }

        [Fact]
        public async Task UpdateLeadAsync_throws_ArgumentException_when_CustomerId_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                Name = "Single Lead",
                Source = "Single Lead Source",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateLeadAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("CustomerId is required", result.Message);
        }

        [Fact]
        public async Task UpdateLeadAsync_throws_ArgumentException_when_name_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",                
                Source = "Single Lead Source",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateLeadAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Name is required", result.Message);
        }

        [Fact]
        public async Task UpdateLeadAsync_throws_ArgumentException_when_source_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",
                Name = "Single Lead",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateLeadAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Source is required", result.Message);
        }
  

        [Fact]
        public async Task UpdateLeadAsync_throws_ArgumentException_when_status_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",
                Name = "Single Lead",
                Source = "Single Lead Source",              
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateLeadAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Status is required", result.Message);
        }

        [Fact]
        public async Task UpdateLeadAsync_returns_a_valid_customer()
        {
            _mockLeadRepository.Setup(c => c.GetLeadById(It.IsAny<string>())).Returns(_mockSingleLead);
            _mockLeadRepository.Setup(c => c.SaveLeadAsync(It.IsAny<LeadModel>())).ReturnsAsync(_mockSingleLead);

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await service.UpdateLeadAsync(_mockSingleLead.Id!, _mockSingleLead);
            Assert.True(result);

            _mockLeadRepository.Verify(r => r.GetLeadById(It.IsAny<string>()), Times.Once);
            _mockLeadRepository.Verify(r => r.SaveLeadAsync(_mockSingleLead), Times.Once);
        }

        [Fact]
        public async Task UpdateLeadAsync_throws_LeadNotFoundException_wheno_customer_is_found()
        {

            _mockLeadRepository.Setup(c => c.GetLeadById(It.IsAny<string>())).Returns<LeadModel>(null);

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<LeadNotFoundException>(async () => await service.UpdateLeadAsync(_mockSingleLead.Id, _mockSingleLead));

            Assert.NotNull(result);
            Assert.Equal($"Lead not found for id {_mockSingleLead.Id}", result.Message);
        }




        /// <summary>
        /// ///////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <returns></returns>


        [Fact]
        public async Task CreateLeadAsync_throws_ArgumentException_when_CustomerId_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                Name = "Single Lead",
                Source = "Single Lead Source",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateLeadAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("CustomerId is required", result.Message);
        }

        [Fact]
        public async Task CreateLeadAsync_throws_ArgumentException_when_name_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",
                Source = "Single Lead Source",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateLeadAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Name is required", result.Message);
        }

        [Fact]
        public async Task CreateLeadAsync_throws_ArgumentException_when_source_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",
                Name = "Single Lead",
                Status = Enums.LeadStatus.CLOSED_WON,
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateLeadAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Source is required", result.Message);
        }

        [Fact]
        public async Task CreateLeadAsync_throws_ArgumentException_when_status_is_not_provided()
        {
            var customerModel = new LeadModel
            {
                Id = "12345",
                CustomerId = "11111",
                Name = "Single Lead",
                Source = "Single Lead Source",
                CreatedDateUtc = DateTime.UtcNow,
            };

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateLeadAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Status is required", result.Message);
        }

        [Fact]
        public async Task CreateLeadAsync_returns_a_valid_customer()
        {
            _mockLeadRepository.Setup(c => c.SaveLeadAsync(It.IsAny<LeadModel>())).ReturnsAsync(_mockSingleLead);

            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await service.CreateLeadAsync(_mockSingleLead);
            Assert.NotNull(result);
            Assert.Equal(_mockSingleLead, result);                        
            _mockLeadRepository.Verify(r => r.SaveLeadAsync(_mockSingleLead), Times.Once);
        }
 

        [Fact]
        public async Task CreateLeadAsync_returns_a_valid_lead()
        {
            _mockLeadRepository.Setup(c => c.SaveLeadAsync(It.IsAny<LeadModel>())).ReturnsAsync(_mockSingleLead);
            var service = new LeadsService(_mockLeadRepository.Object);
            var result = await service.CreateLeadAsync(_mockSingleLead);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.True(result.CreatedDateUtc < DateTime.UtcNow);
            _mockLeadRepository.Verify(r => r.SaveLeadAsync(_mockSingleLead), Times.Once);
        }

        public void Dispose()
        {
            _mockLeadRepository.Reset();
        }
    }
}
