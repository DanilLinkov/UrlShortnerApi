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
            return await _authService.Login(userLoginDto);
        }
        
        [HttpPost("register")]
        public async Task<ApiResponse> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            return await _authService.Register(userRegisterDto);
        }
        
        [HttpPost("logout")]
        public async Task<ApiResponse> Logout()
        {
            return await _authService.Logout();
        }
        
    }
}