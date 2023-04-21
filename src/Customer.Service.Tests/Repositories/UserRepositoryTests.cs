namespace Customer.Service.Tests.Repositories
{
    public class UserRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<UserModel>> _collection;

        private readonly IEnumerable<UserModel> _mockUsers = new List<UserModel>
        {
            new UserModel
            {
                Id = "71204C8F-7E5C-49E3-9932-2EA81134E30E",
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDateUtc = DateTime.UtcNow,
            },
            new UserModel
            {
                Id = "81304C8F-7E5C-49E3-7756-2EA81137A18P",
                Email = "other.user@mock.com",
                FirstName = "Other",
                LastName = "UserTestSurname",
                Password = "foo_bar_fred",
                Salt = "ate_some_bread",
                CreatedDateUtc = DateTime.UtcNow,
            },
        };

        public UserRepositoryTests()
        {
            _collection = new Mock<IDocumentCollection<UserModel>>();
        }

        [Fact]
        public void CheckEmailAvailability_calls_returns_false_if_email_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var result = userRepo.CheckEmailAvailability("test.user@mock.com");

            Assert.False(result);
        }

        [Fact]
        public void CheckEmailAvailability_calls_returns_true_if_email_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var result = userRepo.CheckEmailAvailability("new.user@mock.com");

            Assert.True(result);
        }

        [Fact]
        public async Task CreateUser_calls_InsertOneAsync_when_adding_a_new_record()
        {
            var newUser = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDateUtc = DateTime.UtcNow,
            };

            var userRepo = new UserRepository(_collection.Object);

            var result = await userRepo.CreateUserAsync(newUser);
            _collection.Verify(c => c.InsertOneAsync(newUser), Times.Once());
        }

        [Fact]
        public void GetUser_returns_a_valid_user_if_one_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var result = userRepo.GetUser("71204C8F-7E5C-49E3-9932-2EA81134E30E");

            Assert.NotNull(result);
            Assert.Equal("71204C8F-7E5C-49E3-9932-2EA81134E30E", result.Id);
            Assert.Equal("Test", result.FirstName);
            Assert.Equal("TestSurname", result.LastName);
            Assert.Equal("test.user@mock.com", result.Email);
            Assert.Equal("password", result.Password);
            Assert.Equal("passwordSaltString", result.Salt);
        }

        [Fact]
        public void GetUser_returns_null_if_no_match_is_found_for_id()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var result = userRepo.GetUser("786786786");
            Assert.Null(result);
        }

        [Fact]
        public async Task EditUserAsync_calls_ReplaceOneAsync_when_updating_a_user_record()
        {
            var editedUser = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDateUtc = DateTime.UtcNow,
            };
            _collection.Setup(c => c.AsQueryable()).Returns(new List<UserModel> { editedUser });

            var userRepo = new UserRepository(_collection.Object);
            var result = await userRepo.EditUserAsync(editedUser);

            _collection.Verify(c => c.ReplaceOneAsync(editedUser.Id, editedUser, false), Times.Once());
        }

        [Fact]
        public async Task EditUserAsync_throws_UserNotFoundException_when_existing_user_record_is_not_found()
        {
            var editedUser = new UserModel
            {
                Id = Guid.NewGuid().ToString(),
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDateUtc = DateTime.UtcNow,
            };
            _collection.Setup(c => c.AsQueryable()).Returns(new List<UserModel>());

            var userRepo = new UserRepository(_collection.Object);

            await Assert.ThrowsAsync<UserNotFoundException>(() => userRepo.EditUserAsync(editedUser));
            _collection.Verify(r => r.AsQueryable(), Times.Once);
        }  

        [Fact]
        public void GetUserByEmail_returns_a_user_matching_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var user = userRepo.GetUserByEmail("test.user@mock.com");

            Assert.NotNull(user);
            Assert.IsType<UserModel>(user);
            Assert.Equal("71204C8F-7E5C-49E3-9932-2EA81134E30E", user.Id);
            Assert.Equal("Test", user.FirstName);
            Assert.Equal("TestSurname", user.LastName);
            Assert.Equal("passwordSaltString", user.Salt);
            Assert.Equal("test.user@mock.com", user.Email);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetUserByEmail_returns_null_when_no_user_is_found_to_match_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_mockUsers);

            var userRepo = new UserRepository(_collection.Object);
            var user = userRepo.GetUserByEmail("abcdef1234567890");

            Assert.Null(user);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        public void Dispose()
        {
            _collection.Reset();
        }
    }
}
