using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoWrapper.Interface;
using AutoWrapper.Models;
using Microsoft.AspNetCore.Identity;
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
        public async Task<IApiResponse> Login([FromBody] UserLoginDto userLoginDto)
        {
            var user = await _authService.GetUser(userLoginDto.Username);

            if (user == null)
            {
                HttpContext.Response.StatusCode = 404;
                return new ApiResponse("User not found", null, 404);
            }

            var loginUser = await _authService.Login(user, userLoginDto.Password);

            if (loginUser is null)
            {
                HttpContext.Response.StatusCode = 401;
                return new ApiResponse("Invalid password",null, 401);
            }
            
            return new ApiResponse("Login successful",loginUser, 200);
        }
        
        [HttpGet("login")]
        public async Task<IApiResponse> Login()
        {
            try
            {
                var loginUser = await _authService.ValidateSession();
                
                return new ApiResponse("Logged in", loginUser, 200);
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = 401;
                return new ApiResponse("Not logged in", null, 401);
            }
        }
        
        [HttpPost("register")]
        public async Task<IApiResponse> Register([FromBody] UserRegisterDto userRegisterDto)
        {
            var registerResult = await _authService.Register(userRegisterDto);
            
            if (!registerResult.Succeeded)
            {
                var dictionary = new Dictionary<string, string>();
                foreach (IdentityError error in registerResult.Errors)
                {
                    dictionary.Add(error.Code, error.Description);
                }
                
                HttpContext.Response.StatusCode = 400;
                return new ApiResponse("Invalid user data provided",  dictionary, 400);
            }
            
            return new ApiResponse("User registered successfully",null,  200);
        }
        
        [HttpPost("logout")]
        public async Task<IApiResponse> Logout()
        {
            await _authService.Logout();
            
            return new ApiResponse("Logout successful",null, 200);
        }
        
    }
}