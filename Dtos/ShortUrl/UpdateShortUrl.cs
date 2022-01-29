using System;

namespace UrlShortner.Dtos.ShortUrl
{
    public class UpdateShortUrl
    {
        public string ShortenedUrlId { get; set; }
        public string LongUrl { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}