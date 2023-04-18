﻿using Customer.Service.Exceptions;
using Customer.Service.Models;

namespace Customer.Service.Services
{
    public class LoginService: ILoginService
    {
        private readonly IUserService _userService;

        public LoginService(ISessionService sessionService, IUserService userService)
            _sessionService = sessionService;

        public async Task<string?> Login(LoginRequest request)
        {
            if(request == null) throw new ArgumentNullException(nameof(request));
            if(string.IsNullOrEmpty(request.Email)) throw new ArgumentException($"{nameof(request.Email)} is required");
            if(string.IsNullOrEmpty(request.Password)) throw new ArgumentException($"{nameof(request.Password)} is required");

            var user = _userService.GetUserByEmail(request.Email);
            if(user == null) throw new UserNotFoundException($"User not found for {request.Email}");

            var passwordValid = _userService.ValidateUserPassword(user.Id, request.Password);
            if(!passwordValid) throw new InvalidPasswordException("Password is not valid");

            return await _sessionService.CreateSession(user.Id);
        }

        public async Task<bool> Logout() => await _sessionService.DeleteCurrentSession();
    }
}