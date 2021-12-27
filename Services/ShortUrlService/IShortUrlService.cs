using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Models;
using AutoWrapper.Wrappers;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public interface IShortUrlService
    {
        Task<ApiResponse> GetAllShortUrls();
        Task<ApiResponse> GetShortUrl(string shortUrl);
        Task<ApiResponse> CreateShortUrl(CreateShortUrl shortUrl);
        Task<ApiResponse> UpdateShortUrl(UpdateShortUrl shortUrl); //UpdateShortUrl
        Task<ApiResponse> DeleteShortUrl(string shortUrl); //List<GetShortUrlDto>
    }
}