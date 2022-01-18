using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using UrlShortner.Dtos.User;
using UrlShortner.Services.CacheService;

namespace UrlShortner.Services.AuthService
{
    public class AnonymousAuthService : IAnonymousAuthService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheService _cacheService;

        public AnonymousAuthService(IHttpContextAccessor httpContentAccessor, ICacheService cacheService)
        {
            _httpContextAccessor = httpContentAccessor ?? throw new ArgumentNullException(nameof(httpContentAccessor));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        public async Task<ApiResponse> LoginAnonymousUser()
        {
            var idCookie = GetAnonymousUserId();
            
            if (string.IsNullOrEmpty(idCookie))
            {
                var anonymousUserId = Guid.NewGuid();

                CreateAnonymousIdCookie(anonymousUserId);
                await CreateAnonymousSessionCookie(anonymousUserId);
            }
            else
            {
                var anonymousUserId = Guid.Parse(idCookie);
                await CreateAnonymousSessionCookie(anonymousUserId);
            }
            
            return new ApiResponse(StatusCodes.Status200OK, "Success");
        }

        public Task<ApiResponse> LogoutAnonymousUser()
        {
            var idCookie = GetAnonymousUserId();

            if (!string.IsNullOrEmpty(idCookie))
            {
                _cacheService.Remove(idCookie);
            }
            
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("ShortUrl-GUID");
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("ShortUrl-Session");

            return Task.FromResult(new ApiResponse(StatusCodes.Status200OK, "Success"));
        }
        
        private string GetAnonymousUserId()
        {
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("ShortUrl-GUID", out var idCookie);
            return idCookie;
        }

        private void CreateAnonymousIdCookie(Guid anonymousUserId)
        {
            var anonymousIdCookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(1),
                Secure = true
            };
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append("ShortUrl-GUID", anonymousUserId.ToString(), anonymousIdCookie);
        }
        
        private async Task CreateAnonymousSessionCookie(Guid anonymousUserId)
        {
            var sessionCookie = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(1),
                Secure = true
            };
            
            var sessionId = Guid.NewGuid();

            var anonymousUserInfoDict = new Dictionary<string, string>()
            {
                ["UserId"] = anonymousUserId.ToString(),
                ["dateCreated"] = DateTime.Now.ToString(),
                ["ip"] = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
            };
            
            await _cacheService.Set($"anonymousSessionId-${sessionId}", anonymousUserInfoDict);
            
            // Encrypt cookie
            
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append("ShortUrl-SessionId", sessionId.ToString(), sessionCookie);
        }
    }
}