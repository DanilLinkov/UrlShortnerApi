using System;
using System.Threading.Tasks;

namespace UrlShortner.Decryptors
{
    public interface IDecryptor
    {
        Task<String> DecryptAsync(String cipherText);
    }
}