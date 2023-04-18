using Customer.Service.Exceptions;
using Customer.Service.Infrastructure.Auth;
using Customer.Service.Models;using Customer.Service.Repositories;namespace Customer.Service.Services{    public class SessionService: ISessionService    {
        private readonly IConfiguration _configuration;        private readonly IUserService _userService;        private readonly ISessionRepository _sessionRepository;        private readonly RequestContext _requestContext;
        private readonly ITokenHelper _tokenHelper;        public SessionService(RequestContext requestContext, ITokenHelper tokenHelper, ISessionRepository sessionRepository, IConfiguration configuration, IUserService userService)        {            _tokenHelper = tokenHelper;            _requestContext = requestContext;            _configuration = configuration;
            _sessionRepository = sessionRepository;            _userService = userService;        }        public async Task<string?> CreateSession(string userId)        {            var sessionObect = CreateSessionObject(userId);
            var session = await _sessionRepository.CreateSession(sessionObect);
            return session.Token;
        }

        public async Task<bool> DeleteAllSessionForCurrentUser()        {
            if(string.IsNullOrEmpty(_requestContext.UserId)) throw new UnauthorizedAccessException();
            return await _sessionRepository.DeleteAllSessionForUser(_requestContext.UserId);        }        public async Task<bool> DeleteSession(string id)        {
            return await _sessionRepository.DeleteSession(id);        }

        public async Task<bool> DeleteCurrentSession()        {
            if(string.IsNullOrEmpty(_requestContext.Token)) throw new UnauthorizedAccessException();
            return await _sessionRepository.DeleteCurrentSession(_requestContext.Token!);        }        public IEnumerable<Session> GetAll()        {
            return _sessionRepository.GetAll();        }

        public Session? GetSession(string id)
        {
            return _sessionRepository.GetSession(id);
        }

        public bool IsTokenActive()        {
            if(string.IsNullOrEmpty(_requestContext.Token)) return false;
            return _tokenHelper.IsActive(_requestContext.Token);
        }

        private Session CreateSessionObject(string userId)        {            var user = _userService.GetUser(userId);            if(user == null)            {
                throw new UserNotFoundException($"User not found for id {userId}");
            }            return new Session            {                Id = Guid.NewGuid().ToString(),                Token = _tokenHelper.CreateJwtToken(_configuration, user),
                UserId = userId,                UserEmail = user.Email,                CreatedDate = DateTime.UtcNow,
            };
        }    }}