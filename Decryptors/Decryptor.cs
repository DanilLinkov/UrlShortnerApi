using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortner.Decryptors
{
    public class Decryptor : IDecryptor
    {
        private readonly string _secret;

        public Decryptor(string secret)
        {
            _secret = secret ?? throw new ArgumentNullException(nameof(secret));
        }
        
        public Task<string> DecryptAsync(string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_secret);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return Task.FromResult(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }
    }
}