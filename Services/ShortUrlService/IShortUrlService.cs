using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Models;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public interface IShortUrlService
    {
        Task<List<GetShortUrlDto>> GetAllShortUrls();
        Task<GetShortUrlDto> GetShortUrl(string shortUrl);
        Task<GetShortUrlDto> CreateShortUrl(CreateShortUrl shortUrl);
        Task<GetShortUrlDto> UpdateShortUrl(UpdateShortUrl shortUrl); //UpdateShortUrl
        Task<List<GetShortUrlDto>> DeleteShortUrl(string shortUrl); //List<GetShortUrlDto>
    }
}