using System.Collections.Generic;
using UrlShortner.Models;

namespace UrlShortner.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public List<ShortUrl> ShortUrls { get; set; }
    }
}