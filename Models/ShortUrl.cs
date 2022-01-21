using System;
using UrlShortner.Data;
using UrlShortner.Models.Auth;

namespace UrlShortner.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortenedUrlId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public Guid UserId { get; set; }
        public int Uses { get; set; }
    }
}