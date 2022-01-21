using System;

namespace UrlShortner.Dtos.ShortUrl
{
    public class GetShortUrlDto
    {
        public string LongUrl { get; set; }
        public string ShortenedUrl { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public int Uses { get; set; }
    }
}