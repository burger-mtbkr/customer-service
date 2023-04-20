namespace Customer.Service.Tests.Repositories
{
    public class CustomerRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<CustomerModel>> _collection;

        private readonly CustomerModel _mockSingleCustomer = new()
        {
            Id = "1",
            Company = "Spidertracks",
            CreatedDateUtc = DateTime.UtcNow,
            Email = "test@test.com",
            FirstName = "Customer",
            LastName = "Smith",
            PhoneNumber = "0211034226",
            Status = Enums.CustomerStatus.ACTIVE
        };

        private readonly IEnumerable<CustomerModel> _mockCustomerList;

        public CustomerRepositoryTests()
        {
            _collection = new Mock<IDocumentCollection<CustomerModel>>();
            _mockCustomerList = new List<CustomerModel>
            {
                _mockSingleCustomer,
                new CustomerModel
                {
                    Id = "2",
                    Company = "Spidertracks",
                    CreatedDateUtc = DateTime.UtcNow,
                    Email = "foo@bar.com",
                    FirstName = "Foo",
                    LastName = "Far",
                    PhoneNumber = "0211034226",
                    Status = Enums.CustomerStatus.NON_ACTIVE
                },
                new CustomerModel
                {
                    Id = "3",
                    Company = "Spidertracks",
                    CreatedDateUtc = DateTime.UtcNow,
                    Email = "foo@bar.com",
                    FirstName = "Foo",
                    LastName = "Far",
                    PhoneNumber = "0211034226",
                    Status = Enums.CustomerStatus.LEAD
                }
            };
        }

        [Fact]
        public void GetAllCustomers_returns_all_stored_customer_records()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(3, result.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_returns_empty_list_when_none_are_found()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(new List<CustomerModel>());
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers();

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Empty(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetCustomerByID_returns_a_valid_customer_if_one_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);

            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetCustomerByID("1");

            Assert.NotNull(result);
            Assert.IsAssignableFrom<CustomerModel>(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetCustomerByID_returns_null_if_no_match_is_found_for_id()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);

            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetCustomerByID("10");

            Assert.Null(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public async Task DeleteCustomerAsync_calls_DeleteOneAsync_with_provided_customer()
        {
            _collection.Setup(c => c.DeleteOneAsync(It.IsAny<string>())).ReturnsAsync(true);

            var customerRepo = new CustomerRepository(_collection.Object);
            await customerRepo.DeleteCustomerAsync(_mockSingleCustomer);
            _collection.Verify(c => c.DeleteOneAsync(_mockSingleCustomer.Id), Times.Once());
        }

        [Fact]
        public async Task SaveCustomerAsync_calls_ReplaceOneAsync_when_updating_a_user_record()
        {
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = await customerRepo.SaveCustomerAsync(_mockSingleCustomer);
            _collection.Verify(c => c.ReplaceOneAsync(_mockSingleCustomer.Id, _mockSingleCustomer, true), Times.Once());
        }

        public void Dispose()
        {
            _collection.Reset();
        }
    }
}
