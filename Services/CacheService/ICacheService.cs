using System.Threading.Tasks;

namespace UrlShortner.Services.CacheService
{
    public interface ICacheService
    {
        public Task<T> GetAsync<T>(string key);
        public Task SetAsync<T>(string key, T value); 
        
        public Task RemoveAsync(string key);
    }
}