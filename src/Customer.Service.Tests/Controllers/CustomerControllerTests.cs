namespace Customer.Service.Tests.Controllers
{
    public  class CustomerControllerTests: IDisposable
    {

        private Mock<ICustomerService> _mockCustomerService;    
      
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

        public CustomerControllerTests()
        {
            _mockCustomerService = new Mock<ICustomerService>();
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
        public void Get_returns_valid_ok_response()
        {
            _mockCustomerService.Setup(s => s.GetAllCustomers()).Returns(_mockCustomerList);

            var controller = new CustomerController(_mockCustomerService.Object);
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
            _mockCustomerService.Setup(s => s.GetAllCustomers()).Returns(_mockCustomerList);

            var controller = new CustomerController(_mockCustomerService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<IEnumerable<CustomerModel>>(result.Value);
        }

        [Fact]
        public void Get_returns_all_stored_customers()
        {
            _mockCustomerService.Setup(s => s.GetAllCustomers()).Returns(_mockCustomerList);

            var controller = new CustomerController(_mockCustomerService.Object);
            var response = controller.Get();

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var items = (IEnumerable<CustomerModel>)result.Value!;
            Assert.Equal(3, items.Count());
        }

        [Fact]
        public void Get_by_id_returns_valid_ok_response()
        {
            _mockCustomerService.Setup(s => s.GetCustomerByID(It.IsAny<string>())).Returns(_mockSingleCustomer);

            var controller = new CustomerController(_mockCustomerService.Object);
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
            _mockCustomerService.Setup(s => s.GetCustomerByID(It.IsAny<string>())).Returns(_mockCustomerList.First());

            var controller = new CustomerController(_mockCustomerService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));

            Assert.NotNull(result.Value);
            Assert.IsAssignableFrom<CustomerModel>(result.Value);
        }

        [Fact]
        public void Get_by_id_returns_customer_object()
        {
            _mockCustomerService.Setup(s => s.GetCustomerByID(It.IsAny<string>())).Returns(_mockCustomerList.First());

            var controller = new CustomerController(_mockCustomerService.Object);
            var response = controller.Get("id");

            Assert.NotNull(response);
            Assert.IsType<OkObjectResult>(response.Result);

            var result = (Assert.IsType<OkObjectResult>(response.Result));
            var item = (CustomerModel)result.Value!;
            Assert.NotNull(item);
        }

        [Fact]
        public async Task Post_returns_a_new_valid_customer_with_valid_request()
        {
            _mockCustomerService.Setup(s => s.CreateCustomerAsync(_mockSingleCustomer)).ReturnsAsync(_mockSingleCustomer);

            var controller = new CustomerController(_mockCustomerService.Object);
            var response = await controller.Post(_mockSingleCustomer);
            Assert.NotNull(response);
            Assert.IsType<CreatedResult>(response.Result);

            var result = (CreatedResult)response.Result;
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            Assert.Equal("/api/customer", result.Location);
            Assert.Equal(_mockSingleCustomer, result.Value);
        }


        [Fact]
        public async Task Put_returns_no_content_when_updating_a_customer_by_id()
        {
            _mockCustomerService.Setup(s => s.UpdateCustomerAsync(_mockSingleCustomer.Id, _mockSingleCustomer)).ReturnsAsync(true);
            
            var controller = new CustomerController(_mockCustomerService.Object);
            var response = await controller.Put(_mockSingleCustomer.Id, _mockSingleCustomer);
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Delete_returns_no_content_when_deleting_a_customer_by_id()
        {
            _mockCustomerService.Setup(s => s.DeleteCustomerAsync(It.IsAny<string>())).ReturnsAsync(true);
            var controller = new CustomerController(_mockCustomerService.Object);
            var response = await controller.Delete("id");
            Assert.IsType<NoContentResult>(response);
        }

        [Fact]
        public async Task Patch_returns_no_content_when_updating_customer_password()
        {
            _mockCustomerService.Setup(s => s.UpdateCustomerStatusAsync(It.IsAny<string>(), It.IsAny<CustomerStatusUpdateRequest>())).ReturnsAsync(true);
            var controller = new CustomerController(_mockCustomerService.Object);
            var response = await controller.Patch(It.IsAny<string>(), It.IsAny<CustomerStatusUpdateRequest>());
            Assert.IsType<AcceptedResult>(response);
        }

        public void Dispose()
        {
            _mockCustomerService.Reset();
        }
    }
}
