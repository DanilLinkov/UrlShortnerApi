using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using UrlShortner.Data;

namespace UrlShortner.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(string username, string password);
        Task<ApiResponse> Register(User user, string password);
        Task<ApiResponse> Logout();
        Task<bool> UserExists(string username);
    }
}