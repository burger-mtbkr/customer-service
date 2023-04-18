using Customer.Service.Exceptions;
using Customer.Service.Models;
using Customer.Service.Repositories;
using JsonFlatFileDataStore;

namespace Customer.Service.UnitTests.Repositories
{
    public class UserRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<UserModel>> _collection;

        private readonly IEnumerable<UserModel> _users = new List<UserModel>
        {
            new UserModel
            {
                Id = "71204C8F-7E5C-49E3-9932-2EA81134E30E",
                Email = "test.user@mock.com",
                FirstName = "Test",
                LastName = "TestSurname",
                Password = "password",
                Salt = "passwordSaltString",
                CreatedDate = DateTime.UtcNow,
            },
            new UserModel
            {
                Id = "81304C8F-7E5C-49E3-7756-2EA81137A18P",
                Email = "other.user@mock.com",
                FirstName = "Other",
                LastName = "UserTestSurname",
                Password = "foo_bar_fred",
                Salt = "ate_some_bread",
                CreatedDate = DateTime.UtcNow,
            },
        };

        public UserRepositoryTests()
        {
            _collection = new Mock<IDocumentCollection<UserModel>>();
        }

        [Fact]
        public void CheckEmailAvailability_calls_returns_false_if_email_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var result = sessionRepo.CheckEmailAvailability("test.user@mock.com");

            Assert.False(result);
        }

        [Fact]
        public void CheckEmailAvailability_calls_returns_true_if_email_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var result = sessionRepo.CheckEmailAvailability("new.user@mock.com");

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
                CreatedDate = DateTime.UtcNow,
            };

            var sessionRepo = new UserRepository(_collection.Object);

            var result = await sessionRepo.CreateUserAsync(newUser);
            _collection.Verify(c => c.InsertOneAsync(newUser), Times.Once());
        }

        [Fact]
        public void GetUser_returns_a_valid_user_if_one_exists()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var result = sessionRepo.GetUser("71204C8F-7E5C-49E3-9932-2EA81134E30E");

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
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var result = sessionRepo.GetUser("786786786");
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
                CreatedDate = DateTime.UtcNow,
            };
            _collection.Setup(c => c.AsQueryable()).Returns(new List<UserModel> { editedUser });

            var sessionRepo = new UserRepository(_collection.Object);
            var result = await sessionRepo.EditUserAsync(editedUser);

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
                CreatedDate = DateTime.UtcNow,
            };
            _collection.Setup(c => c.AsQueryable()).Returns(new List<UserModel>());

            var userRepo = new UserRepository(_collection.Object);

            await Assert.ThrowsAsync<UserNotFoundException>(() => userRepo.EditUserAsync(editedUser));
            _collection.Verify(r => r.AsQueryable(), Times.Once);
        }

        [Fact]
        public void GetAllUsers_returns_all_stored_users()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);
            var usersRepo = new UserRepository(_collection.Object);
            var users = usersRepo.GetAllUsers();

            Assert.NotNull(users);
            Assert.IsAssignableFrom<IEnumerable<UserModel>>(users);
            Assert.Equal(2, users.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetUserByEmail_returns_a_session_matching_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var user = sessionRepo.GetUserByEmail("test.user@mock.com");

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
        public void GetUserByEmail_returns_null_when_no_session_is_found_to_match_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var sessionRepo = new UserRepository(_collection.Object);
            var session = sessionRepo.GetUserByEmail("abcdef1234567890");

            Assert.Null(session);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public async Task DeleteUserAsync_calls_DeleteOneAsync_with_provided_userId()
        {
            var id = "71204C8F-7E5C-49E3-9932-2EA81134E30E";
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var userRepo = new UserRepository(_collection.Object);
            await userRepo.DeleteUserAsync(id);

            _collection.Verify(c => c.AsQueryable(), Times.Once());
            _collection.Verify(c => c.DeleteOneAsync(It.IsAny<UserModel>()), Times.Once());
        }

        [Fact]
        public async Task DeleteUserAsync_throws_UserNotFoundException_whne_user_is_not_found()
        {
            var id = "test-7E5C-49E3-9932-2EA81134E30E";
            _collection.Setup(c => c.AsQueryable()).Returns(_users);

            var userRepo = new UserRepository(_collection.Object);
            await Assert.ThrowsAsync<UserNotFoundException>(() => userRepo.DeleteUserAsync(id));
            _collection.Verify(r => r.AsQueryable(), Times.Once);
        }

        public void Dispose()
        {
            _collection.Reset();
        }
    }
}
