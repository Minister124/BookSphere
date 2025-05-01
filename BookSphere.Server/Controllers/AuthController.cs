using BookSphere.DTOs;
using BookSphere.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookSphere.Controllers
{
    [Route("Auth")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register(RegisterDto registerDto)
        {
            var response = await _userService.RegisterAsync(registerDto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(LoginDto loginDto)
        {
            var response = await _userService.LoginAsync(loginDto);
            return Ok(response);
        }
    }
}
