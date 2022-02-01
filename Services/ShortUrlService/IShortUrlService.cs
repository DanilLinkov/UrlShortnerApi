using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Models;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public interface IShortUrlService
    {
        Task<List<GetShortUrlDto>> GetAllShortUrlsAsync();
        Task<GetShortUrlDto> GetShortUrlAsync(string shortUrl);
        Task<GetShortUrlDto> CreateShortUrlAsync(CreateShortUrl shortUrl);
        Task<GetShortUrlDto> UpdateShortUrlAsync(UpdateShortUrl shortUrl); //UpdateShortUrl
        Task<List<GetShortUrlDto>> DeleteShortUrlAsync(DeleteShortUrlDto shortUrl); //List<GetShortUrlDto>
    }
}