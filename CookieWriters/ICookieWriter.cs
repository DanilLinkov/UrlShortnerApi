using System.Threading.Tasks;

namespace UrlShortner.CookieWriters
{
    public interface ICookieWriter
    {
        Task WriteCookie(string cookieName, string cookieValue);
    }
}