using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using UrlShortner.Data;
using UrlShortner.Dtos.User;
using UrlShortner.Models.Auth;

namespace UrlShortner.Services.AuthService
{
    public interface IAuthService
    {
        Task<GetUserDto> LoginAsync(User user, string password);
        Task<IdentityResult> RegisterAsync(UserRegisterDto user);
        Task<GetUserDto> ValidateSessionAsync();
        Task LogoutAsync();
        Task<User> GetUserAsync(string userName);
    }
}