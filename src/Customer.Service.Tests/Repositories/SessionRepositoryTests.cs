using Customer.Service.Models;
using Customer.Service.Repositories;
using JsonFlatFileDataStore;
using Microsoft.Extensions.Logging;

namespace Customer.Service.UnitTests.Repositories
{
    public class SessionRepositoryTests: IDisposable
    {
        private readonly Mock<IDocumentCollection<Session>> _collection;
        private readonly Mock<ILogger<SessionRepository>> _mockLogger;

        private readonly IEnumerable<Session> _sessions = new List<Session>
        {
            new Session
            {
                Id = Guid.NewGuid().ToString(),
                CreatedDate = DateTime.UtcNow,
                Token = Guid.NewGuid().ToString(),
                UserEmail = "some.test@user.com",
                UserId = Guid.NewGuid().ToString(),
                Expiry = DateTime.UtcNow.AddHours(720),
            },
            new Session {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                CreatedDate = DateTime.UtcNow,
                Token = "boohoo123456778",
                UserEmail = "some.test@user.com",
                UserId = "B15A0836-BCBF-49DC-83E7-0F9D962C2A79",
                Expiry = DateTime.UtcNow.AddHours(720),
            }
        };

        public SessionRepositoryTests()
        {
            //_mockDataStore = new Mock<IDataStore>();
            _mockLogger = new Mock<ILogger<SessionRepository>>();
            _collection = new Mock<IDocumentCollection<Session>>();
        }

        [Fact]
        public async Task CreateSession_calls_insertOneAsync_with_session_object()
        {
            var sessionObject = new Session
            {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                CreatedDate = DateTime.UtcNow,
                Token = "boohoo123456778",
                UserEmail = "some.test@user.com",
                UserId = "B15A0836-BCBF-49DC-83E7-0F9D962C2A79",
                Expiry = DateTime.UtcNow.AddHours(720),
            };

            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            await sessionRepo.CreateSession(sessionObject);

            _collection.Verify(c => c.InsertOneAsync(sessionObject), Times.Once());
        }

        [Fact]
        public void GetAll_returns_all_stored_sessions()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_sessions);
            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            var sessions = sessionRepo.GetAll();

            Assert.NotNull(sessions);
            Assert.Equal(2, sessions.Count());
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetSession_returns_a_session_matching_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_sessions);

            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            var session = sessionRepo.GetSession("C51989A0-4D7B-4532-A05C-3851ABE24206");

            Assert.NotNull(session);
            Assert.IsType<Session>(session);
            Assert.Equal("C51989A0-4D7B-4532-A05C-3851ABE24206", session.Id);
            Assert.Equal("boohoo123456778", session.Token);
            Assert.Equal("B15A0836-BCBF-49DC-83E7-0F9D962C2A79", session.UserId);
            Assert.Equal("some.test@user.com", session.UserEmail);

            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public void GetSession_returns_null_when_no_session_is_found_to_match_a_provided_token()
        {
            _collection.Setup(c => c.AsQueryable()).Returns(_sessions);

            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            var session = sessionRepo.GetSession("abcdef1234567890");

            Assert.Null(session);
            _collection.Verify(c => c.AsQueryable(), Times.Once());
        }

        [Fact]
        public async Task DeleteAllSessionForUser_calls_DeleteManyAsync_with_provided_token()
        {
            var testUserId = "B15A0836-BCBF-49DC-83E7-0F9D962C2A79";

            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            await sessionRepo.DeleteAllSessionForUser(testUserId);

            _collection.Verify(c => c.DeleteManyAsync(It.IsAny<Predicate<Session>>()), Times.Once());
        }

        [Fact]
        public async Task DeleteSession_calls_DeleteOneAsync_with_provided_token()
        {
            var id = "some_user_id";

            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            await sessionRepo.DeleteSession(id);

            _collection.Verify(c => c.DeleteOneAsync(It.IsAny<Predicate<Session>>()), Times.Once());
        }

        [Fact]
        public async Task DeleteCurrentSession_calls_DeleteOneAsync_with_provided_token()
        {
            var token = "some_token";
            var sessionRepo = new SessionRepository(_collection.Object, _mockLogger.Object);
            await sessionRepo.DeleteCurrentSession(token);
            _collection.Verify(c => c.DeleteOneAsync(It.IsAny<Predicate<Session>>()), Times.Once());
        }

        public void Dispose()
        {
            _collection.Reset();
            _mockLogger.Reset();
        }
    }
}
