namespace Customer.Service.Tests.Repositories
{
    public class LeadRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<LeadModel>> _collection;

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

        public LeadRepositoryTests()
        {
            _collection = new Mock<IDocumentCollection<LeadModel>>();
        }

        [Fact]
        public void GetLeads_returns_all_stored_leads_for_a_customer()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockLeads);
            var leadsRepository = new LeadRepository(_collection.Object);
            var result = leadsRepository.GetLeads(_mockLeads.First().CustomerId);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<LeadModel>>(result);
            Assert.Equal(1, result.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetLeads_returns_empty_list_when_none_are_found()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(new List<LeadModel>());
            var leadsRepository = new LeadRepository(_collection.Object);
            var result = leadsRepository.GetLeads(It.IsAny<string>());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<LeadModel>>(result);
            Assert.Empty(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetLeadByid_returns_a_valid_customer_if_one_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockLeads);

            var sessionRepo = new LeadRepository(_collection.Object);
            var result = sessionRepo.GetLeadByid(_mockLeads.First().Id);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<LeadModel>(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetLeadByid_returns_null_if_no_match_is_found_for_id()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockLeads);

            var sessionRepo = new LeadRepository(_collection.Object);
            var result = sessionRepo.GetLeadByid("9999");

            Assert.Null(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public async Task SaveCustomerAsync_calls_ReplaceOneAsync_when_updating_a_user_record()
        {
            var leadsRepository = new LeadRepository(_collection.Object);
            var result = await leadsRepository.SaveLeadAsync(_mockLeads.First());
            _collection.Verify(c => c.ReplaceOneAsync(_mockLeads.First().Id, _mockLeads.First(), true), Times.Once());
        }

        public void Dispose()
        {
            _collection.Reset();
        }
    }
}
