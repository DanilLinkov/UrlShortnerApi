using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using UrlShortner.CookieReaders;
using UrlShortner.CookieWriters;
using UrlShortner.Services.CacheService;
using UrlShortner.Util;

namespace UrlShortner.MiddleWares
{
    public class AddAnonymousCookies
    {
        private readonly RequestDelegate _next;

        public AddAnonymousCookies(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }
        
        public async Task InvokeAsync(HttpContext context, ICookieReader cookieReader, ICookieWriter cookieWriter, ICacheService cacheService)
        {
            if (!context.User.Identity.IsAuthenticated)
            {
                var anonymousUserId = await cookieReader.ReadCookie(CookieNames.ShortUrlId);

                var newUserId = false;
                if (string.IsNullOrEmpty(anonymousUserId))
                {
                    newUserId = true;
                    
                    anonymousUserId = Guid.NewGuid().ToString();
                    await cookieWriter.WriteCookie(CookieNames.ShortUrlId, anonymousUserId);
                }
                
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, anonymousUserId)
                };
                var identity = new ClaimsIdentity(claims, "Anonymous");
                    
                context.User.AddIdentity(identity);

                var sessionId = await cookieReader.ReadCookie(CookieNames.ShortUrlSession);

                if (string.IsNullOrEmpty(sessionId) || newUserId)
                {
                    sessionId = Guid.NewGuid().ToString(); 
                    var anonymousUserInfoDict = new Dictionary<string, string>()
                    {
                        ["UserId"] = anonymousUserId,
                        ["dateCreated"] = DateTime.Now.ToString(),
                        ["ip"] = context.Connection.RemoteIpAddress.ToString(),
                    };
                
                    await cacheService.Set($"anonymousSessionId-${sessionId}", anonymousUserInfoDict);
                    await cookieWriter.WriteCookie(CookieNames.ShortUrlSession, sessionId);
                }
            }
            
            await _next(context);
        }
    }
    
    public static class AddAnonymousCookiesMiddlewareExtensions
    {
        public static IApplicationBuilder UseAddAnonymousCookies(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AddAnonymousCookies>();
        }
    }
}