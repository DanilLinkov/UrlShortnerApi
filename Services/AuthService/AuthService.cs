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
using UrlShortner.AuthUserAccessors;
using UrlShortner.Data;
using UrlShortner.Dtos.User;
using UrlShortner.Models.Auth;
using UrlShortner.Util;

namespace UrlShortner.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<User> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IAuthUserAccessor _authUserAccessor;
        private readonly DataContext _dbContext;
        private readonly UserManager<User> _userManager;

        public AuthService(DataContext dbContext, UserManager<User> userManager, SignInManager<User> signInManager,
            IHttpContextAccessor contextAccessor, IAuthUserAccessor authUserAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
            _authUserAccessor = authUserAccessor ?? throw new ArgumentNullException(nameof(authUserAccessor));
        }

        public async Task<GetUserDto> LoginAsync(User user, string password)
        {
            var result =
                _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                    password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (!_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var anonUserGuid = await _authUserAccessor.GetAuthUserIdAsync();
                await TransferAnonUserShortUrls(anonUserGuid, user);
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

        public async Task<IdentityResult> RegisterAsync(UserRegisterDto user)
        {
            Guid? anonUserGuid = null;
            if (!_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                anonUserGuid = await _authUserAccessor.GetAuthUserIdAsync();
            }

            var newUser = new User() {UserName = user.Username};
            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded)
            {
                await TransferAnonUserShortUrls(anonUserGuid, newUser);
                
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                    new(ClaimTypes.Name, newUser.UserName)
                };

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await _signInManager.SignInWithClaimsAsync(newUser, authProperties, claims);
            }

            return result;
        }

        private async Task TransferAnonUserShortUrls(Guid? anonUserGuid, User newUser)
        {
            if (anonUserGuid != null)
            {
                var shortUrlsToTransfer = await _dbContext.ShortUrls
                    .Where(s => s.UserId == anonUserGuid && s.ExpirationDate > DateTime.Now).ToListAsync();
                shortUrlsToTransfer.ForEach(s => s.UserId = newUser.Id);

                await _dbContext.SaveChangesAsync();
            }
        }

        public Task<GetUserDto> ValidateSessionAsync()
        {
            if (_contextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return Task.FromResult(new GetUserDto()
                {
                    UserName = _contextAccessor.HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Name)
                        .FirstOrDefault().Value,
                });
            }

            return null;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<User> GetUserAsync(string userName)
        {
            var existingUser = await _userManager.FindByNameAsync(userName);

            return existingUser;
        }
    }
}