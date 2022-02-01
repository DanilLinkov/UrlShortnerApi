using System.Threading.Tasks;

namespace UrlShortner.KeyGenerators
{
    public interface IKeyGenerator
    {
        Task<string> GenerateKeyAsync(int size);
        
        Task<string[]> GenerateKeyAsync(int count, int size);
    }
}