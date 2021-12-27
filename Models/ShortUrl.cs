using System;

namespace UrlShortner.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string LongUrl { get; set; }
        public string ShortenedUrl { get; set; }
        public DateTime CreationDate { get; } = DateTime.Now;
        public DateTime ExpirationDate { get; set; }
    }
}