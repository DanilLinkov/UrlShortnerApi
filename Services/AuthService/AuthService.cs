using System;
using System.Collections.Generic;
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
        private readonly UserManager<User> _userManager;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }
        
        public async Task<bool> Login(User user, string password)
        {
            var result =
                _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                    password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                return false;
            }
            
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _signInManager.SignInWithClaimsAsync(user, authProperties, claims);
            
            return true;
        }

        public async Task<IdentityResult> Register(UserRegisterDto user)
        {
            var newUser = new User() {UserName = user.Username};
            var result = await _userManager.CreateAsync(newUser, user.Password);

            return result;
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