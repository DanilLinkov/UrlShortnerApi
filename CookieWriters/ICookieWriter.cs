using System;
using System.Threading.Tasks;

namespace UrlShortner.CookieWriters
{
    public interface ICookieWriter
    {
        Task WriteCookieAsync(string cookieName, string cookieValue);
        
        Task WriteCookieAsync(string cookieName, string cookieValue, DateTime expiration);
    }
}