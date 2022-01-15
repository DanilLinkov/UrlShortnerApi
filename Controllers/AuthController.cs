using System;
using System.Threading.Tasks;
using AutoWrapper.Wrappers;
using Microsoft.AspNetCore.Mvc;
using UrlShortner.Data;
using UrlShortner.Dtos.User;
using UrlShortner.Services.AuthService;

namespace UrlShortner.Controllers
{
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }
        
        [HttpPost("login")]
        public async Task<ApiResponse> Login([FromBody] UserLoginDto userLoginDto)
        {
            var result = await _authService.Login(userLoginDto.Username, userLoginDto.Password);
            return result;
        }

        [HttpPost("register")]
        public async Task<ApiResponse> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var result = await _authService.Register(new User { Username = userRegisterDto.Username }, userRegisterDto.Password);
            return result;
        }
        
        [HttpPost("logout")]
        public async Task<ApiResponse> Logout()
        {
            var result = await _authService.Logout();
            return result;
        }
        
    }
}