using System;
using System.Threading.Tasks;
using AutoWrapper.Exceptions;
using AutoWrapper.Interface;
using AutoWrapper.Models;
using Microsoft.AspNetCore.Mvc;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;
using UrlShortner.Services.ShortUrlService;

namespace UrlShortner.Controllers
{
    [ApiController]
    [Route("api")]
    public class ShortUrlController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public ShortUrlController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService ?? throw new ArgumentNullException(nameof(shortUrlService));
        }
        
        [HttpGet]
        public async Task<IApiResponse> GetAll()
        {
            var shortUrlsResult = await _shortUrlService.GetAllShortUrls();
            
            return new ApiResponse(shortUrlsResult);
        }
        
        [HttpGet("{shortUrl}")]
        public async Task<IApiResponse> Get(string shortUrl)
        {
            var shortUrlResult = await _shortUrlService.GetShortUrl(shortUrl);

            if (shortUrlResult == null)
            {
                HttpContext.Response.StatusCode = 404;
                return new ApiResponse("Short url not found",null, 404);
            }

            return new ApiResponse(shortUrlResult); 
        }
        
        [HttpPost]
        public async Task<ApiResponse> Create([FromBody] CreateShortUrl shortUrl)
        {
            var newShortUrlResult = await _shortUrlService.CreateShortUrl(shortUrl);

            if (newShortUrlResult == null)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResponse("Key generation service is down", null, 400);
            }

            return new ApiResponse(newShortUrlResult);
        }
        
        [HttpPut]
        public async Task<ApiResponse> Update([FromBody] UpdateShortUrl shortUrl)
        {
            var updatedShortUrlResult = await _shortUrlService.UpdateShortUrl(shortUrl);
            
            return new ApiResponse(updatedShortUrlResult);
        }
        
        [HttpDelete]
        public async Task<ApiResponse> Delete([FromBody] DeleteShortUrlDto shortUrl)
        {
            var deletedShortUrlResult = await _shortUrlService.DeleteShortUrl(shortUrl);
            
            return new ApiResponse(deletedShortUrlResult);
        }
        
    }
}