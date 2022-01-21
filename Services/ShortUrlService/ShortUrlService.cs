using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AutoWrapper.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using UrlShortner.AuthUserAccessors;
using UrlShortner.Data;
using UrlShortner.Dtos.ShortUrl;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IAuthUserAccessor _authUserAccessor;

        public ShortUrlService(IMapper mapper, DataContext context, IAuthUserAccessor authUserAccessor)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authUserAccessor = authUserAccessor ?? throw new ArgumentNullException(nameof(authUserAccessor));
        }

        public async Task<List<GetShortUrlDto>> GetAllShortUrls()
        {
            var userId = await _authUserAccessor.GetAuthUserId();
                
            var shortUrlsResults = await _context.ShortUrls.Where(s => s.UserId.Equals(userId)).ToListAsync();
            var shortUrlsResultDtoList = shortUrlsResults.Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();
                
            return shortUrlsResultDtoList;
        }

        public async Task<GetShortUrlDto> GetShortUrl(string shortUrl)
        {
            var shortUrlResult = await _context.ShortUrls
                .Where(s => s.ShortenedUrlId.Equals(shortUrl))
                .FirstOrDefaultAsync();
            
            if (shortUrlResult == null)
            {
                return null;
            }

            shortUrlResult.Uses++;
            
            await _context.SaveChangesAsync();
            
            var shortUrlResultDto = _mapper.Map<GetShortUrlDto>(shortUrlResult);

            return shortUrlResultDto;
        }

        public async Task<GetShortUrlDto> CreateShortUrl(CreateShortUrl shortUrl)
        {
            try
            {
                var userId = await _authUserAccessor.GetAuthUserId();

                var newShortUrl = _mapper.Map<ShortUrl>(shortUrl);

                newShortUrl.UserId = userId;
                
                if (shortUrl.ExpirationDate == null)
                {
                    newShortUrl.ExpirationDate = DateTime.Now + TimeSpan.FromDays(7);
                }
                
                newShortUrl.CreationDate = DateTime.Now;
                newShortUrl.ShortenedUrlId = Guid.NewGuid().ToString().Substring(0, 6);

                await _context.ShortUrls.AddAsync(newShortUrl);
                await _context.SaveChangesAsync();

                var newShortUrlDto = _mapper.Map<GetShortUrlDto>(newShortUrl);
                
                return newShortUrlDto;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<GetShortUrlDto> UpdateShortUrl(UpdateShortUrl shortUrl)
        {
            try
            {
                var userId = await _authUserAccessor.GetAuthUserId();
                
                var shortUrlToUpdate = await _context.ShortUrls.Where(s => s.UserId.Equals(userId) && s.ShortenedUrlId.Equals(shortUrl.ShortenedUrl)).FirstOrDefaultAsync();
                
                if (shortUrlToUpdate == null)
                {
                    return null;
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
                
                return shortUrlDto;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<List<GetShortUrlDto>> DeleteShortUrl(string shortUrl)
        {
            try
            {
                var userId = await _authUserAccessor.GetAuthUserId();
                
                var shortUrlToDelete = await _context.ShortUrls.Where(s => s.UserId.Equals(userId) && s.ShortenedUrlId.Equals(HttpUtility.UrlDecode(shortUrl))).FirstOrDefaultAsync();
                
                if (shortUrlToDelete == null)
                {
                    return null;
                }
                
                _context.ShortUrls.Remove(shortUrlToDelete);
                await _context.SaveChangesAsync();
                
                var shortUrlDto = (await _context.ShortUrls.ToListAsync()).Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();
                
                return shortUrlDto;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}