using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlShortner.CookieReaders
{
    public interface ICookieReader
    {
        Task<string> ReadCookieAsync(string cookieName);
    }
}