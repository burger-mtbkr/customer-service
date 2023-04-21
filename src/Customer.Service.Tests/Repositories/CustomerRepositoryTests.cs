namespace Customer.Service.Tests.Repositories
{
    public class CustomerRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<CustomerModel>> _collection;

        private readonly CustomerModel _mockSingleCustomer = new()
        {
            Id = "1",
            Company = "Microsoft",
            CreatedDateUtc = DateTime.UtcNow,
            Email = "bill.gates@ms.com",
            FirstName = "Bill",
            LastName = "Gates",
            PhoneNumber = "11111111111",
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
                    Company = "Apple",
                    CreatedDateUtc = DateTime.UtcNow,
                    Email = "steve.jobs@icloud.com",
                    FirstName = "Steve",
                    LastName = "Jobs",
                    PhoneNumber = "2222222222",
                    Status = Enums.CustomerStatus.NON_ACTIVE
                },
                new CustomerModel
                {
                    Id = "3",
                    Company = "Google",
                    CreatedDateUtc = DateTime.UtcNow,
                    Email = "larry.page@gmail.com",
                    FirstName = "Larry Page",
                    LastName = "Far",
                    PhoneNumber = "3333333333",
                    Status = Enums.CustomerStatus.LEAD
                },
                 new CustomerModel
                {
                    Id = "4",
                    Company = "Twitter",
                    CreatedDateUtc = DateTime.UtcNow,
                    Email = "elon@twitter.com",
                    FirstName = "Elon",
                    LastName = "Musk",
                    PhoneNumber = "444444444",
                    Status = Enums.CustomerStatus.LEAD
                },
            };
        }

        [Fact]
        public void GetAllCustomers_returns_all_stored_customer_records()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_returns_empty_list_when_none_are_found()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(new List<CustomerModel>());
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

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

        [Fact]
        public void GetAllCustomers_with_search_string_returns_correct_data()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), "steve", null);

            Assert.NotNull(result);

            var expectedSearchResultCustomer = _mockCustomerList.First(n => n.FirstName == "Steve");

            Assert.Equal(expectedSearchResultCustomer, result.First());
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Single(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_sortField_asc_returns_correct_data_sored()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "asc");

            Assert.NotNull(result);

            Assert.Equal("Bill", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_sortField_desc_returns_correct_data_sored()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "desc");

            Assert.NotNull(result);

            Assert.Equal("Steve", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_without_sortField_returns_correct_data_sored_asc()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("", "asc");

            Assert.NotNull(result);

            Assert.Equal("Bill", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }


        [Fact]
        public void GetAllCustomers_without_sortField_returns_correct_data_sored_desc()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("", "desc");

            Assert.NotNull(result);

            Assert.Equal("Steve", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_incorrect_sortField_returns_correct_data_sored_asc()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("jhkhkj", "asc");

            Assert.NotNull(result);

            Assert.Equal("Bill", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }


        [Fact]
        public void GetAllCustomers_with_incorrect_sortField_returns_correct_data_sored_desc()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("jhjhgj", "desc");

            Assert.NotNull(result);

            Assert.Equal("Steve", result.First().FirstName);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_active_statuFilter_returns_correct_data()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "asc", "", 0);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Single(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_lead_statuFilter_returns_correct_data()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "asc", "", 1);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(2, result.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_non_active_statuFilter__returns_correct_data()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "asc", "", 2);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Single(result);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetAllCustomers_with_incorrect_active_statuFilter_returns_all__data()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockCustomerList);
            var customerRepo = new CustomerRepository(_collection.Object);
            var result = customerRepo.GetAllCustomers("firstName", "asc", "", 5);

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result);
            Assert.Equal(4, result.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        public void Dispose()
        {
            _collection.Reset();
        }
    }
}
