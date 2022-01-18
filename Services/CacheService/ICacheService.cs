using System.Threading.Tasks;

namespace UrlShortner.Services.CacheService
{
    public interface ICacheService
    {
        public Task<T> Get<T>(string key);
        public Task Set<T>(string key, T value); 
        
        public Task Remove(string key);
    }
}