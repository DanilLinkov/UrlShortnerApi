using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using UrlShortner.Dtos.User;

namespace UrlShortner.Services.AuthService
{
    public interface IAnonymousAuthService
    {
        Task<ApiResponse> LoginAnonymousUser();
        Task<ApiResponse> LogoutAnonymousUser();
    }
}