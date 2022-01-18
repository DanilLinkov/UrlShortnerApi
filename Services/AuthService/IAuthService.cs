using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using UrlShortner.Data;
using UrlShortner.Dtos.User;

namespace UrlShortner.Services.AuthService
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(UserLoginDto user);
        Task<ApiResponse> Register(UserRegisterDto user);
        Task<ApiResponse> Logout();
    }
}