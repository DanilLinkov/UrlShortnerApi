using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AutoWrapper.Models;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShortner.Data;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public ShortUrlService(IMapper mapper, DataContext context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ApiResponse> GetAllShortUrls()
        {
            try
            {
                var shortUrlsResult = await _context.ShortUrls.ToListAsync();

                var shortUrlsResultDto = shortUrlsResult.Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();
                
                return new ApiResponse(shortUrlsResultDto);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ApiResponse> GetShortUrl(string shortUrl)
        {
            try
            {
                var shortUrlResult = await _context.ShortUrls.Where(s => s.ShortenedUrl.Equals(HttpUtility.UrlDecode(shortUrl))).FirstOrDefaultAsync();
                
                var shortUrlResultDto = _mapper.Map<GetShortUrlDto>(shortUrlResult);

                return shortUrlResult == null ? new ApiResponse("Provided short url was not found.",StatusCodes.Status404NotFound) : new ApiResponse(shortUrlResultDto);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ApiResponse> CreateShortUrl(CreateShortUrl shortUrl)
        {
            try
            {
                var newShortUrl = _mapper.Map<ShortUrl>(shortUrl);

                if (shortUrl.ExpirationDate == null)
                {
                    newShortUrl.ExpirationDate = DateTime.Now + TimeSpan.FromDays(7);
                }
                
                newShortUrl.ShortenedUrl = $"https://localhost:3000/${Guid.NewGuid().ToString().Substring(0, 6)}";

                await _context.ShortUrls.AddAsync(newShortUrl);
                await _context.SaveChangesAsync();

                var newShortUrlDto = _mapper.Map<GetShortUrlDto>(newShortUrl);
                
                return new ApiResponse(newShortUrlDto);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ApiResponse> UpdateShortUrl(UpdateShortUrl shortUrl)
        {
            try
            {
                var shortUrlToUpdate = await _context.ShortUrls.Where(s => s.ShortenedUrl.Equals(shortUrl.ShortenedUrl)).FirstOrDefaultAsync();
                
                if (shortUrlToUpdate == null)
                {
                    return new ApiResponse("Provided short url was not found.", StatusCodes.Status404NotFound);
                }
                
                shortUrlToUpdate.LongUrl = shortUrl.LongUrl;
                shortUrlToUpdate.ExpirationDate = shortUrl.ExpirationDate;
                
                if (shortUrlToUpdate.ExpirationDate > DateTime.Now + TimeSpan.FromDays(30))
                {
                    shortUrlToUpdate.ExpirationDate = DateTime.Now + TimeSpan.FromDays(30);
                }

                _context.ShortUrls.Update(shortUrlToUpdate);
                await _context.SaveChangesAsync();
                
                var shortUrlDto = _mapper.Map<GetShortUrlDto>(shortUrlToUpdate);
                
                return new ApiResponse(shortUrlDto);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<ApiResponse> DeleteShortUrl(string shortUrl)
        {
            try
            {
                var shortUrlToDelete = await _context.ShortUrls.Where(s => s.ShortenedUrl.Equals(HttpUtility.UrlDecode(shortUrl))).FirstOrDefaultAsync();
                
                if (shortUrlToDelete == null)
                {
                    return new ApiResponse("Provided short url was not found.", StatusCodes.Status404NotFound);
                }
                
                _context.ShortUrls.Remove(shortUrlToDelete);
                await _context.SaveChangesAsync();
                
                var shortUrlDto = (await _context.ShortUrls.ToListAsync()).Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();
                
                return new ApiResponse(shortUrlDto);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}