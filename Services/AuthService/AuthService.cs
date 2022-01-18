using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
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
        
        public async Task<ApiResponse> Login(UserLoginDto user)
        {
            var existingUser = await _userManager.FindByNameAsync(user.Username);

            if (existingUser == null)
            {
                return new ApiResponse(StatusCodes.Status404NotFound, "User with the provided credentials was not found");
            }

            var result =
                _userManager.PasswordHasher.VerifyHashedPassword(existingUser, existingUser.PasswordHash,
                    user.Password);
            
            if (result == PasswordVerificationResult.Failed)
            {
                return new ApiResponse(StatusCodes.Status401Unauthorized, "User with the provided credentials was not found");
            }
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, existingUser.Id.ToString())
            };

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await _signInManager.SignInWithClaimsAsync(existingUser, authProperties, claims);
            
            return new ApiResponse(StatusCodes.Status200OK, "User successfully logged in");
        }

        public async Task<ApiResponse> Register(UserRegisterDto user)
        {
            var newUser = new User() {UserName = user.Username};
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (!result.Succeeded)
            {
                var dictionary = new Dictionary<string, string>();
                foreach (IdentityError error in result.Errors)
                {
                    dictionary.Add(error.Code, error.Description);
                }
                
                return new ApiResponse("Invalid user data provided",  dictionary, 400);
            }
            
            return new ApiResponse("User registered successfully",  200);
        }

        public async Task<ApiResponse> Logout()
        {
            await _signInManager.SignOutAsync();
            return new ApiResponse("User successfully logged out",  200);
        }
    }
}