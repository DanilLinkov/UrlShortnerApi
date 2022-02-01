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
            var shortUrlsResult = await _shortUrlService.GetAllShortUrlsAsync();
            
            return new ApiResponse(shortUrlsResult);
        }
        
        [HttpGet("{shortUrl}")]
        public async Task<IApiResponse> Get(string shortUrl)
        {
            var shortUrlResult = await _shortUrlService.GetShortUrlAsync(shortUrl);

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
            GetShortUrlDto newShortUrlResult = null;
            
            try
            {
                newShortUrlResult = await _shortUrlService.CreateShortUrlAsync(shortUrl);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResponse(e.Message, null, 400);
            }

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
            GetShortUrlDto updatedShortUrlResult = null;
            
            try
            {
                updatedShortUrlResult = await _shortUrlService.UpdateShortUrlAsync(shortUrl);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = 400;
                return new ApiResponse(e.Message, null, 400);
            }
            
            if (updatedShortUrlResult == null)
            {
                HttpContext.Response.StatusCode = 404;
                return new ApiResponse("ShortUrl with the provided Id was not found", null, 404);
            }

            return new ApiResponse(updatedShortUrlResult);
        }
        
        [HttpDelete]
        public async Task<ApiResponse> Delete([FromBody] DeleteShortUrlDto shortUrl)
        {
            var deletedShortUrlResult = await _shortUrlService.DeleteShortUrlAsync(shortUrl);

            if (deletedShortUrlResult == null)
            {
                HttpContext.Response.StatusCode = 404;
                return new ApiResponse("ShortUrl with the provided Id was not found", null, 404);
            }

            return new ApiResponse(deletedShortUrlResult);
        }
        
    }
}