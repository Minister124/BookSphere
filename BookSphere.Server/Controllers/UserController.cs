using BookSphere.DTOs;
using BookSphere.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookSphere.Controllers
{
    [Route("User")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ActionResult<UserDto>> GetUserProfile()
        {
            var userId = GetUserID();
        }
    }
}
