using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlShortner.CookieReaders
{
    public class CookieReaderToResponse : ICookieReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CookieReaderToResponse(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        public Task<string> ReadCookie(string cookieName)
        {
            // TODO Decrypt cookie
            
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue);

            return Task.FromResult(cookieValue);
        }
    }
}