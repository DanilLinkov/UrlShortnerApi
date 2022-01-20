using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlShortner.CookieWriters
{
    public class CookieWriterToResponse : ICookieWriter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CookieOptions _cookieOptions;

        public CookieWriterToResponse(IHttpContextAccessor httpContextAccessor, CookieOptions cookieOptions)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _cookieOptions = cookieOptions ?? throw new ArgumentNullException(nameof(cookieOptions));
        }
        
        public Task WriteCookie(string cookieName, string cookieValue)
        {
            // TODO Encryption of cookie value
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, cookieValue, _cookieOptions);

            return Task.CompletedTask;
        }
    }
}