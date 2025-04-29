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
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }


        [HttpGet("getprofilebyid")]
        public async Task<ActionResult<UserDto>> GetUserProfile()
        {
            var userId = GetUserId();
            var user = await _userService.GetUserProfileAsync(userId);

            return Ok(user);
        }

        [HttpPut("update/profile")]
        public async Task<ActionResult<UserDto>> UpdateUserProfile(UserDto userDto)
        {
            var userId = GetUserId();
            var user = await _userService.UpdateUserProfileAsync(userId, userDto);
            return Ok(user);
        }

        [HttpPost("role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AssignRole(Guid userId, [FromBody] string role)
        {
            await _userService.AssignRoleAsync(userId, role);
            return NoContent();
        }

        [HttpGet("discount/count")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> GetSuccessfulOrderCount()
        {
            
        }
    }
}
