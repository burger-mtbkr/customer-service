﻿using Customer.Service.Exceptions;
using Customer.Service.Infrastructure.Auth;
using Customer.Service.Models;
        private readonly IConfiguration _configuration;
        private readonly ITokenHelper _tokenHelper;
            _sessionRepository = sessionRepository;
            var session = await _sessionRepository.CreateSession(sessionObect);
            return session.Token;
        }

        public async Task<bool> DeleteAllSessionForCurrentUser()
            if(string.IsNullOrEmpty(_requestContext.UserId)) throw new UnauthorizedAccessException();
            return await _sessionRepository.DeleteAllSessionForUser(_requestContext.UserId);
            return await _sessionRepository.DeleteSession(id);

        public async Task<bool> DeleteCurrentSession()
            if(string.IsNullOrEmpty(_requestContext.Token)) throw new UnauthorizedAccessException();
            return await _sessionRepository.DeleteCurrentSession(_requestContext.Token!);
            return _sessionRepository.GetAll();

        public Session? GetSession(string id)
        {
            return _sessionRepository.GetSession(id);
        }

        public bool IsTokenActive()
            if(string.IsNullOrEmpty(_requestContext.Token)) return false;
            return _tokenHelper.IsActive(_requestContext.Token);
        }

        private Session CreateSessionObject(string userId)
                throw new UserNotFoundException($"User not found for id {userId}");
            }
                UserId = userId,
            };
        }