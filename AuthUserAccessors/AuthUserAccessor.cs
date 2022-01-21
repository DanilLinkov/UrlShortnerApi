using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlShortner.AuthUserAccessors
{
    public class AuthUserAccessor : IAuthUserAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthUserAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }
        
        public Task<Guid> GetAuthUserId()
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            return Task.FromResult(Guid.Parse(userId.Value));
        }
    }
}