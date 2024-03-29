﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using UrlShortner.Decryptors;

namespace UrlShortner.CookieReaders
{
    public class CookieReaderToResponse : ICookieReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDecryptor _decryptor;

        public CookieReaderToResponse(IHttpContextAccessor httpContextAccessor, IDecryptor decryptor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _decryptor = decryptor ?? throw new ArgumentNullException(nameof(decryptor));
        }
        
        public async Task<string> ReadCookieAsync(string cookieName)
        {
            _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(cookieName, out var cookieValue);
            
            if (!string.IsNullOrEmpty(cookieValue))
            {
                try
                {
                    return await _decryptor.DecryptAsync(cookieValue);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            return cookieValue;
        }
    }
}