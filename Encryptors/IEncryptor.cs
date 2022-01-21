using System;
using System.Threading.Tasks;

namespace UrlShortner.Encryptors
{
    public interface IEncryptor
    {
        Task<String> Encrypt(String plainText);
    }
}