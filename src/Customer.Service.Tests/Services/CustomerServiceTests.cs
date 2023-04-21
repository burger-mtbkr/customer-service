namespace Customer.Service.Tests.Services
{
    public class CustomerServiceTests: IDisposable
    {
        private Mock<ICustomerRepository> _mockCustomerRepository;

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

        public CustomerServiceTests()
        {
            _mockCustomerRepository = new Mock<ICustomerRepository>();
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
        public void GetAllCustomers_calls_GetAllCustomers_repository()
        {
            _mockCustomerRepository.Setup(u => u.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(_mockCustomerList);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = service.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
            _mockCustomerRepository.Verify(r => r.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetAllCustomers_returns_empty_list_when_none_are_found()
        {
            _mockCustomerRepository.Setup(u => u.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).Returns(new List<CustomerModel>());

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = service.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());

            Assert.NotNull(result);
            Assert.Empty(result);
            _mockCustomerRepository.Verify(r => r.GetAllCustomers(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetCustomerByID_returns_customer_matching_id()
        {
            _mockCustomerRepository.Setup(c => c.GetCustomerByID(_mockSingleCustomer.Id)).Returns(_mockSingleCustomer);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = service.GetCustomerByID(_mockSingleCustomer.Id);

            Assert.NotNull(result);
            Assert.Equal(_mockSingleCustomer, result);

            _mockCustomerRepository.Verify(r => r.GetCustomerByID(_mockSingleCustomer.Id), Times.Once);
        }

        [Fact]
        public void GetCustomerByID_throws_ArgumentNullException_when_id_is_null()
        {
            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = Assert.Throws<ArgumentNullException>(() => service.GetCustomerByID(null));
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        [Fact]
        public void GetCustomerByID_throws_ArgumentNullException_when_id_is_empty()
        {
            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = Assert.Throws<ArgumentNullException>(() => service.GetCustomerByID(string.Empty));
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        [Fact]
        public void GetCustomerByID_throws_CustomerNotFoundException_when_id_does_not_match_any_customers()
        {
            var id = "5648787786";
            _mockCustomerRepository.Setup(c => c.GetCustomerByID(It.IsAny<string>())).Returns<CustomerModel>(null);
            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = Assert.Throws<CustomerNotFoundException>(() => service.GetCustomerByID(id));
            _mockCustomerRepository.Verify(r => r.GetCustomerByID(id), Times.Once);
            Assert.Equal($"Customer not found for id {id}", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_email_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Email is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_name_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "foo@bar.com",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("FirstName is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_lastname_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("LastName is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_phone_number_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("PhoneNumber is required", result.Message);
        }


        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_compamny_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Company is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_ArgumentException_when_status_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerAsync(customerModel.Id, customerModel));

            Assert.NotNull(result);
            Assert.Equal("Status is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerAsync_returns_a_valid_customer()
        {
            _mockCustomerRepository.Setup(c => c.GetCustomerByID(It.IsAny<string>())).Returns(_mockSingleCustomer);
            _mockCustomerRepository.Setup(c => c.SaveCustomerAsync(It.IsAny<CustomerModel>())).ReturnsAsync(_mockSingleCustomer);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await service.UpdateCustomerAsync(_mockSingleCustomer.Id!, _mockSingleCustomer);
            Assert.True(result);

            _mockCustomerRepository.Verify(r => r.GetCustomerByID(It.IsAny<string>()), Times.Once);
            _mockCustomerRepository.Verify(r => r.SaveCustomerAsync(_mockSingleCustomer), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_throws_CustomerNotFoundException_whenno_customer_is_found()
        {

            _mockCustomerRepository.Setup(c => c.GetCustomerByID(It.IsAny<string>())).Returns<CustomerModel>(null);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<CustomerNotFoundException>(async () => await service.UpdateCustomerAsync(_mockSingleCustomer.Id, _mockSingleCustomer));

            Assert.NotNull(result);
            Assert.Equal($"Customer not found for id {_mockSingleCustomer.Id}", result.Message);
        }


        [Fact]
        public async Task UpdateCustomerStatusAsync_throws_ArgumentException_when_id_is_not_provided()
        {
            var statusUpdateRequest = new CustomerStatusUpdateRequest
            {
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(async () => await service.UpdateCustomerStatusAsync("", statusUpdateRequest));

            Assert.NotNull(result);
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerStatusAsync_throws_ArgumentException_when_status_is_not_provided()
        {
            var customerModel = new CustomerStatusUpdateRequest();

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.UpdateCustomerStatusAsync(It.IsAny<string>(), customerModel));

            Assert.NotNull(result);
            Assert.Equal("Status is required", result.Message);
        }

        [Fact]
        public async Task UpdateCustomerStatusAsync_returns_true()
        {
            var statusUpdateRequest = new CustomerStatusUpdateRequest
            {
                Status = Enums.CustomerStatus.ACTIVE
            };

            _mockCustomerRepository.Setup(c => c.GetCustomerByID(It.IsAny<string>())).Returns(_mockSingleCustomer);
            _mockCustomerRepository.Setup(c => c.SaveCustomerAsync(It.IsAny<CustomerModel>())).ReturnsAsync(_mockSingleCustomer);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await service.UpdateCustomerStatusAsync(_mockSingleCustomer.Id!, statusUpdateRequest);
            Assert.True(result);

            _mockCustomerRepository.Verify(r => r.GetCustomerByID(It.IsAny<string>()), Times.Once);
            _mockCustomerRepository.Verify(r => r.SaveCustomerAsync(_mockSingleCustomer), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerStatusAsync_throws_CustomerNotFoundException_when_no_customer_is_found()
        {
            var statusUpdateRequest = new CustomerStatusUpdateRequest
            {
                Status = Enums.CustomerStatus.ACTIVE
            };

            _mockCustomerRepository.Setup(c => c.GetCustomerByID(It.IsAny<string>())).Returns<CustomerModel>(null);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<CustomerNotFoundException>(async () => await service.UpdateCustomerStatusAsync(_mockSingleCustomer.Id, statusUpdateRequest));

            Assert.NotNull(result);
            Assert.Equal($"Customer not found for id {_mockSingleCustomer.Id}", result.Message);
            _mockCustomerRepository.Verify(r => r.GetCustomerByID(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CreateCustomerAsync_throws_ArgumentException_when_email_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Email is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsync_throws_ArgumentException_when_name_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "foo@bar.com",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("FirstName is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsync_throws_ArgumentException_when_lastname_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("LastName is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsyncc_throws_ArgumentException_when_company_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Company is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsyncc_throws_ArgumentException_when_phone_number_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                Status = Enums.CustomerStatus.ACTIVE
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("PhoneNumber is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsync_throws_ArgumentException_when_status_is_not_provided()
        {
            var customerModel = new CustomerModel
            {
                Id = "1",
                Company = "Spidertracks",
                CreatedDateUtc = DateTime.UtcNow,
                Email = "test@test.com",
                FirstName = "Customer",
                LastName = "Smith",
                PhoneNumber = "0211034226",
            };

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentException>(async () => await service.CreateCustomerAsync(customerModel));

            Assert.NotNull(result);
            Assert.Equal("Status is required", result.Message);
        }

        [Fact]
        public async Task CreateCustomerAsync_returns_a_valid_customer()
        {
            _mockCustomerRepository.Setup(c => c.SaveCustomerAsync(It.IsAny<CustomerModel>())).ReturnsAsync(_mockSingleCustomer);
            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await service.CreateCustomerAsync(_mockSingleCustomer);

            Assert.NotNull(result);
            Assert.NotEmpty(result.Id);
            Assert.True(result.CreatedDateUtc < DateTime.UtcNow);
            _mockCustomerRepository.Verify(r => r.SaveCustomerAsync(_mockSingleCustomer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_calls_DeleteUser_on_repository_with_correct_id()
        {
            _mockCustomerRepository.Setup(u => u.GetCustomerByID(_mockSingleCustomer.Id)).Returns(_mockSingleCustomer);
            _mockCustomerRepository.Setup(u => u.DeleteCustomerAsync(_mockSingleCustomer)).ReturnsAsync(true);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await service.DeleteCustomerAsync(_mockSingleCustomer.Id);

            Assert.True(result);
            _mockCustomerRepository.Verify(r => r.DeleteCustomerAsync(_mockSingleCustomer), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_throws_ArgumentNullException_when_id_is_empty()
        {
            var id = "";
            _mockCustomerRepository.Setup(u => u.DeleteCustomerAsync(_mockSingleCustomer)).ReturnsAsync(true);

            var service = new CustomerService(_mockCustomerRepository.Object);
            var result = await Assert.ThrowsAsync<ArgumentNullException>(() => service.DeleteCustomerAsync(id));

            Assert.NotNull(result);
            Assert.Equal("Value cannot be null. (Parameter 'id')", result.Message);
        }

        public void Dispose()
        {
            _mockCustomerRepository.Reset();
        }
    }
}
