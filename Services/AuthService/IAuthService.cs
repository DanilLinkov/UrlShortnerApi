using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UrlShortner.Data;
using UrlShortner.Dtos.User;
using UrlShortner.Models.Auth;

namespace UrlShortner.Services.AuthService
{
    public interface IAuthService
    {
        Task<GetUserDto> Login(User user, string password);
        Task<IdentityResult> Register(UserRegisterDto user);
        Task<GetUserDto> ValidateSession();
        Task Logout();
        Task<User> GetUser(string userName);
    }
}