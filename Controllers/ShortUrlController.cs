using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;
using UrlShortner.Services.ShortUrlService;

namespace UrlShortner.Controllers
{
    [ApiController]
    [Route("api/ShortUrl")]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService ?? throw new ArgumentNullException(nameof(shortUrlService));
        }
        
        [HttpGet]
        public async Task<ApiResponse> GetAll()
        {
            return await _shortUrlService.GetAllShortUrls();
        }
        
        [HttpGet("{shortUrl}")]
        public async Task<ApiResponse> Get(string shortUrl)
        {
            return await _shortUrlService.GetShortUrl(shortUrl);
        }
        
        [HttpPost]
        public async Task<ApiResponse> Create([FromBody] CreateShortUrl shortUrl)
        {
            return await _shortUrlService.CreateShortUrl(shortUrl);
        }
        
        [HttpPut]
        public async Task<ApiResponse> Update([FromBody] UpdateShortUrl shortUrl)
        {
            return await _shortUrlService.UpdateShortUrl(shortUrl);
        }
        
        [HttpDelete("{shortUrl}")]
        public async Task<ApiResponse> Delete(string shortUrl)
        {
            return await _shortUrlService.DeleteShortUrl(shortUrl);
        }
        
    }
}