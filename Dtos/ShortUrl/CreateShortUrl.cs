using System;

namespace UrlShortner.Dtos.ShortUrl
{
    public class CreateShortUrl
    {
        public string CustomId { get; set; }
        public string LongUrl { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}