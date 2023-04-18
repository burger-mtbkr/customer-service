﻿using Customer.Service.Exceptions;
using Customer.Service.Infrastructure.Auth;
using Customer.Service.Models;
using Customer.Service.Repositories;
using Customer.Service.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Customer.Service.UnitTests.Services
{
    public class SessionServiceTests: IDisposable
    {
        private readonly Mock<ILogger<SessionService>> _mockLogger;
        private readonly Mock<ISessionRepository> _mockSessionRepository;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ITokenHelper> _mockTokenHelper;
        private readonly RequestContext _requestContext;


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

        public SessionServiceTests()
        {
            _mockLogger = new Mock<ILogger<SessionService>>();
            _mockSessionRepository = new Mock<ISessionRepository>();
            _mockUserService = new Mock<IUserService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTokenHelper = new Mock<ITokenHelper>();
            _requestContext = new RequestContext();
        }

        [Fact]
        public async Task CreateSession_returns_a_token()
        {
            var mockSession = new Session
            {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                CreatedDate = DateTime.UtcNow,
                Token = "boohoo123456778",
                UserEmail = "some.test@user.com",
                UserId = "B15A0836-BCBF-49DC-83E7-0F9D962C2A79",
                Expiry = DateTime.UtcNow.AddHours(720),
            };

            var mockUser = new UserModel
            {
                Id = "C51989A0-4D7B-4532-A05C-3851ABE24206",
                Email = "",
                FirstName = "Test",
                LastName = "Test",
                CreatedDate = DateTime.UtcNow,
                Password = "abcdef",
                Salt = "86876876"
            };


            _mockTokenHelper.Setup(t => t.CreateJwtToken(_mockConfiguration.Object, mockUser)).Returns(mockSession.Token);
            _mockUserService.Setup(u => u.GetUser("C51989A0-4D7B-4532-A05C-3851ABE24206")).Returns(mockUser);
            _mockSessionRepository.Setup(s => s.CreateSession(It.IsAny<Session>())).ReturnsAsync(mockSession);

            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);

            var token = await sessionService.CreateSession("C51989A0-4D7B-4532-A05C-3851ABE24206");

            Assert.NotNull(token);
            Assert.Equal(token, mockSession.Token);
            _mockUserService.Verify(u => u.GetUser("C51989A0-4D7B-4532-A05C-3851ABE24206"), Times.Once);
            _mockSessionRepository.Verify(u => u.CreateSession(It.IsAny<Session>()), Times.Once);
        }

        [Fact]
        public async Task CreateSession_throws_UserNotFoundException_when_no_user_is_found()
        {
            var id = "5648787786";
            _mockUserService.Setup(u => u.GetUser(It.IsAny<string>())).Returns<UserModel>(null);

            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);

            await Assert.ThrowsAsync<UserNotFoundException>(() => sessionService.CreateSession(id));
            _mockUserService.Verify(r => r.GetUser(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAllSessionForCurrentUser_calls_DeleteAllSessionForUser_on_repository_with_correct_userId()
        {
            var requestContext = new RequestContext
            {
                Token = "token",
                UserId = "123456"
            };
            _mockSessionRepository.Setup(u => u.DeleteAllSessionForUser(requestContext.UserId)).ReturnsAsync(true);

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await sessionService.DeleteAllSessionForCurrentUser();

            Assert.True(result);
            _mockSessionRepository.Verify(r => r.DeleteAllSessionForUser(requestContext.UserId), Times.Once);
        }

        [Fact]
        public async Task DeleteAllSessionForCurrentUser_throws_UnauthorizedAccessException_when_userId_is_null()
        {
            var requestContext = new RequestContext
            {
                Token = "token",
            };

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sessionService.DeleteAllSessionForCurrentUser());

            Assert.NotNull(result);
            Assert.Equal("Attempted to perform an unauthorized operation.", result.Message);
        }

        [Fact]
        public async Task DeleteAllSessionForCurrentUser_throws_UnauthorizedAccessException_when_userId_is_empty()
        {
            var requestContext = new RequestContext
            {
                Token = "token",
                UserId = ""
            };

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sessionService.DeleteAllSessionForCurrentUser());

            Assert.NotNull(result);
            Assert.Equal("Attempted to perform an unauthorized operation.", result.Message);
        }

        [Fact]
        public async Task DeleteSession_calls_DeleteSession_on_repository_with_correct_accesToken()
        {
            var mockToken = "someToken";
            _mockSessionRepository.Setup(u => u.DeleteSession(mockToken)).ReturnsAsync(true);

            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await sessionService.DeleteSession(mockToken);

            Assert.True(result);
            _mockSessionRepository.Verify(r => r.DeleteSession(mockToken), Times.Once);
        }

        [Fact]
        public async Task DeleteCurrentSession_calls_DeleteSession_on_repository_with_correct_accesToken()
        {
            var requestContext = new RequestContext
            {
                Token = "token",
                UserId = "123456"
            };
            _mockSessionRepository.Setup(u => u.DeleteCurrentSession("token")).ReturnsAsync(true);

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await sessionService.DeleteCurrentSession();

            Assert.True(result);
            _mockSessionRepository.Verify(r => r.DeleteCurrentSession("token"), Times.Once);
        }

        [Fact]
        public async Task DeleteCurrentSession_throws_UnauthorizedAccessException_when_token_is_empty()
        {
            var requestContext = new RequestContext
            {
                Token = "",
                UserId = "123456"
            };

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sessionService.DeleteCurrentSession());

            Assert.NotNull(result);
            Assert.Equal("Attempted to perform an unauthorized operation.", result.Message);
        }

        [Fact]
        public async Task DeleteCurrentSession_throws_UnauthorizedAccessException_when_token_is_null()
        {
            var requestContext = new RequestContext
            {
                UserId = "123456"
            };

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = await Assert.ThrowsAsync<UnauthorizedAccessException>(() => sessionService.DeleteCurrentSession());

            Assert.NotNull(result);
            Assert.Equal("Attempted to perform an unauthorized operation.", result.Message);
        }

        [Fact]
        public void GetSession_call_returns_session_object_for_valid_token()
        {
            var mockToken = "someToken";

            var mockSession = new Session
            {
                Id = mockToken,
                CreatedDate = DateTime.UtcNow,
                Token = Guid.NewGuid().ToString(),
                UserEmail = "some.test@user.com",
                UserId = Guid.NewGuid().ToString(),
                Expiry = DateTime.UtcNow.AddHours(720),
            };

            _mockSessionRepository.Setup(u => u.GetSession(mockToken)).Returns(mockSession);

            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var resultSessionObject = sessionService.GetSession(mockToken);

            Assert.NotNull(resultSessionObject);

            Assert.Equal(mockSession.Id, resultSessionObject.Id);
            Assert.Equal(mockSession.CreatedDate, resultSessionObject.CreatedDate);
            Assert.Equal(mockSession.Token, resultSessionObject.Token);
            Assert.Equal(mockSession.UserEmail, resultSessionObject.UserEmail);
            Assert.Equal(mockSession.UserId, resultSessionObject.UserId);
            Assert.Equal(mockSession.Expiry, resultSessionObject.Expiry);

            _mockSessionRepository.Verify(r => r.GetSession(mockToken), Times.Once);
        }

        [Fact]
        public void GetSession_call_returns_null_for_unmatched_token()
        {
            var mockToken = "someToken";

            var mockSession = new Session
            {
                Id = mockToken,
                CreatedDate = DateTime.UtcNow,
                Token = Guid.NewGuid().ToString(),
                UserEmail = "some.test@user.com",
                UserId = Guid.NewGuid().ToString(),
                Expiry = DateTime.UtcNow.AddHours(720),
            };

            _mockSessionRepository.Setup(u => u.GetSession(mockToken)).Returns<Session>(null);

            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var resultSessionObject = sessionService.GetSession(mockToken);

            Assert.Null(resultSessionObject);
            _mockSessionRepository.Verify(r => r.GetSession(mockToken), Times.Once);
        }

        [Fact]
        public void GetAll_call_returns_all_sessions()
        {
            _mockSessionRepository.Setup(u => u.GetAll()).Returns(_sessions);
            var sessionService = new SessionService(_requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var resultSessionObject = sessionService.GetAll();

            Assert.NotNull(resultSessionObject);
            Assert.Equal(2, resultSessionObject.Count());
            _mockSessionRepository.Verify(r => r.GetAll(), Times.Once);
        }

        [Fact]
        public void IsTokenActive_returns_true_if_token_is_not_expired()
        {
            var requestContext = new RequestContext
            {
                Token = "token",
                UserId = "123456"
            };
            _mockTokenHelper.Setup(t => t.IsActive(It.IsAny<string>())).Returns(true);

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = sessionService.IsTokenActive();

            Assert.True(result);
            _mockTokenHelper.Verify(r => r.IsActive("token"), Times.Once);
        }

        [Fact]
        public void IsTokenActive_returns_false_if_token_is_not_active()
        {
            var requestContext = new RequestContext
            {
                Token = "blokin",
                UserId = "123456"
            };
            _mockTokenHelper.Setup(t => t.IsActive(It.IsAny<string>())).Returns(false);

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = sessionService.IsTokenActive();

            Assert.False(result);
            _mockTokenHelper.Verify(r => r.IsActive("blokin"), Times.Once);
        }

        [Fact]
        public void IsTokenActive_returns_false_if_token_is_empty()
        {
            var requestContext = new RequestContext
            {
                Token = "",
                UserId = "123456"
            };
            _mockTokenHelper.Setup(t => t.IsActive(It.IsAny<string>())).Returns(false);

            var sessionService = new SessionService(requestContext, _mockTokenHelper.Object, _mockSessionRepository.Object, _mockConfiguration.Object, _mockUserService.Object);
            var result = sessionService.IsTokenActive();
            Assert.False(result);        
        }

        public void Dispose()
        {
            _mockLogger.Reset();
            _mockSessionRepository.Reset();
            _mockUserService.Reset();
            _mockConfiguration.Reset();
            _mockTokenHelper.Reset();
        }
    }
}
