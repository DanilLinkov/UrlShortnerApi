using System;
using System.Threading.Tasks;

namespace UrlShortner.AuthUserAccessors
{
    public interface IAuthUserAccessor
    {
        Task<string> GetAuthUserId();
    }
}