using System.Threading.Tasks;

namespace UrlShortner.KeyGenerators
{
    public interface IKeyGenerator
    {
        Task<string> GenerateKey(int size);
        
        Task<string[]> GenerateKey(int count, int size);
    }
}