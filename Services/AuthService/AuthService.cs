using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoWrapper.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Data;
using UrlShortner.Dtos.User;
using UrlShortner.Models.Auth;

namespace UrlShortner.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IHttpContextAccessor contextAccessor)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
        }
        
        public async Task<GetUserDto> Login(User user, string password)
        {
            var result =
                _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                    password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName)
            };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);
            
            return new GetUserDto()
            {
                UserName = user.UserName,
            };
        }

        public async Task<IdentityResult> Register(UserRegisterDto user)
        {
            var newUser = new User() {UserName = user.Username};
            var result = await _userManager.CreateAsync(newUser, user.Password);

            return result;
        }

        public Task<GetUserDto> ValidateSession()
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult(new GetUserDto()
                {
                    UserName = _contextAccessor.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Name).FirstOrDefault().Value,
                });
            }

            return null;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<User> GetUser(string userName)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);

            return existingUser;
        }
    }
}