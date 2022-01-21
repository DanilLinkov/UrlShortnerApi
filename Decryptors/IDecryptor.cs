using System;
using System.Threading.Tasks;

namespace UrlShortner.Decryptors
{
    public interface IDecryptor
    {
        Task<String> Decrypt(String cipherText);
    }
}