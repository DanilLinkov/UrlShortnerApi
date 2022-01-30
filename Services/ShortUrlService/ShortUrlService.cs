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
using UrlShortner.KeyGenerators;
using UrlShortner.Models;

namespace UrlShortner.Services.ShortUrlService
{
    public class ShortUrlService : IShortUrlService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IAuthUserAccessor _authUserAccessor;
        private readonly IKeyGenerator _keyGenerator;

        public ShortUrlService(IMapper mapper, DataContext context, IAuthUserAccessor authUserAccessor,
            IKeyGenerator keyGenerator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _authUserAccessor = authUserAccessor ?? throw new ArgumentNullException(nameof(authUserAccessor));
            _keyGenerator = keyGenerator ?? throw new ArgumentNullException(nameof(keyGenerator));
        }

        public async Task<List<GetShortUrlDto>> GetAllShortUrls()
        {
            var userId = await _authUserAccessor.GetAuthUserId();

            var shortUrlsResults = await _context.ShortUrls
                .Where(s => s.UserId.Equals(userId) && s.ExpirationDate > DateTime.Now).ToListAsync();
            var shortUrlsResultDtoList = shortUrlsResults.Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();

            return shortUrlsResultDtoList;
        }

        public async Task<GetShortUrlDto> GetShortUrl(string shortUrl)
        {
            var shortUrlResult = await _context.ShortUrls
                .Where(s => s.ShortenedUrlId.Equals(shortUrl) && s.ExpirationDate > DateTime.Now)
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
            var isValidUrl = IsValidUrl(shortUrl.LongUrl);

            if (!isValidUrl)
            {
                throw new Exception("Invalid URL format provided");
            }

            var userId = await _authUserAccessor.GetAuthUserId();

            var newShortUrl = _mapper.Map<ShortUrl>(shortUrl);

            newShortUrl.UserId = userId;

            if (shortUrl.ExpirationDate == null || shortUrl.ExpirationDate < DateTime.Now.AddDays(1))
            {
                newShortUrl.ExpirationDate = DateTime.Now + TimeSpan.FromDays(1);
            }

            if (shortUrl.ExpirationDate > DateTime.Now.AddDays(30))
            {
                throw new Exception("Expiration date cannot be more than 30 days from now");
            }

            newShortUrl.CreationDate = DateTime.Now;

            if (shortUrl.CustomId != null)
            {
                if (shortUrl.CustomId.Trim().Length < 4)
                {
                    throw new Exception("Custom Id must be at least 4 characters long");
                }
                
                if (await _context.ShortUrls.AnyAsync(s => s.ShortenedUrlId.Equals(shortUrl.CustomId)))
                {
                    throw new Exception("Custom Id already exists");
                }

                newShortUrl.ShortenedUrlId = shortUrl.CustomId;
            }
            else
            {
                var newKey = await _keyGenerator.GenerateKey(8);

                if (newKey == null)
                {
                    return null;
                }

                newShortUrl.ShortenedUrlId = newKey;
            }

            await _context.ShortUrls.AddAsync(newShortUrl);
            await _context.SaveChangesAsync();

            var newShortUrlDto = _mapper.Map<GetShortUrlDto>(newShortUrl);

            return newShortUrlDto;
        }

        public async Task<GetShortUrlDto> UpdateShortUrl(UpdateShortUrl shortUrl)
        {
            var isValidUrl = IsValidUrl(shortUrl.LongUrl);

            if (!isValidUrl)
            {
                throw new Exception("Invalid URL format provided");
            }

            var userId = await _authUserAccessor.GetAuthUserId();

            var shortUrlToUpdate = await _context.ShortUrls.Where(s =>
                s.UserId.Equals(userId) && s.ShortenedUrlId.Equals(shortUrl.ShortenedUrlId) &&
                s.ExpirationDate > DateTime.Now).FirstOrDefaultAsync();

            if (shortUrlToUpdate == null)
            {
                return null;
            }

            shortUrlToUpdate.LongUrl = shortUrl.LongUrl;
            shortUrlToUpdate.ExpirationDate = shortUrl.ExpirationDate;
            
            if (shortUrl.ExpirationDate == null || shortUrl.ExpirationDate < DateTime.Now.AddDays(1))
            {
                shortUrlToUpdate.ExpirationDate = DateTime.Now + TimeSpan.FromDays(1);
            }

            if (shortUrl.ExpirationDate > DateTime.Now.AddDays(30))
            {
                throw new Exception("Expiration date cannot be more than 30 days from now");
            }

            _context.ShortUrls.Update(shortUrlToUpdate);
            await _context.SaveChangesAsync();

            var shortUrlDto = _mapper.Map<GetShortUrlDto>(shortUrlToUpdate);

            return shortUrlDto;
        }

        public async Task<List<GetShortUrlDto>> DeleteShortUrl(DeleteShortUrlDto inputDto)
        {
            var userId = await _authUserAccessor.GetAuthUserId();

            var shortUrl = _mapper.Map<DeleteShortUrlDto>(inputDto);

            var shortUrlToDelete = await _context.ShortUrls.Where(s =>
                s.UserId.Equals(userId) && s.ShortenedUrlId.Equals(HttpUtility.UrlDecode(shortUrl.ShortenedUrlId)) &&
                s.ExpirationDate > DateTime.Now).FirstOrDefaultAsync();

            if (shortUrlToDelete == null)
            {
                return null;
            }

            _context.ShortUrls.Remove(shortUrlToDelete);
            await _context.SaveChangesAsync();

            var shortUrlsResults = await _context.ShortUrls
                .Where(s => s.UserId.Equals(userId) && s.ExpirationDate > DateTime.Now).ToListAsync();
            var shortUrlsResultDtoList = shortUrlsResults.Select(s => _mapper.Map<GetShortUrlDto>(s)).ToList();

            return shortUrlsResultDtoList;
        }

        private static bool IsValidUrl(string url)
        {
            var isValidUrl =
                !url.Contains("localhost:3000") && Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                                                && (uriResult.Scheme == Uri.UriSchemeHttp ||
                                                    uriResult.Scheme == Uri.UriSchemeHttps);
            return isValidUrl;
        }
    }
}