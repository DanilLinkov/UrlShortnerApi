using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlShortner.Encryptors;

namespace UrlShortner.CookieWriters
{
    public class CookieWriterToResponse : ICookieWriter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly CookieOptions _cookieOptions;
        private readonly IEncryptor _encryptor;

        public CookieWriterToResponse(IHttpContextAccessor httpContextAccessor, CookieOptions cookieOptions, IEncryptor encryptor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _cookieOptions = cookieOptions ?? throw new ArgumentNullException(nameof(cookieOptions));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
        }
        
        public async Task WriteCookie(string cookieName, string cookieValue)
        {
            var encryptedCookieValue = await _encryptor.Encrypt(cookieValue);
            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, encryptedCookieValue, _cookieOptions);
        }

        public async Task WriteCookie(string cookieName, string cookieValue, DateTime expiration)
        {
            var encryptedCookieValue = await _encryptor.Encrypt(cookieValue);

            var newCookieOptions = new CookieOptions()
            {
                HttpOnly = _cookieOptions.HttpOnly,
                Expires = expiration,
                Secure = _cookieOptions.Secure,
                SameSite = _cookieOptions.SameSite,
                Path = _cookieOptions.Path,
            };

            _httpContextAccessor.HttpContext.Response.Cookies.Append(cookieName, encryptedCookieValue, newCookieOptions);
        }
    }
}