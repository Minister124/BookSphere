using BookSphere.DTOs;
using BookSphere.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookSphere.Controllers
{
    [Route("WhiteList")]
    [ApiController]
    public class WhiteListController : BaseController
    {
        private readonly IWhiteListService _whiteListService;

        public WhiteListController(IWhiteListService whiteListService)
        {
            _whiteListService = whiteListService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<WhiteListDto>> GetWhiteList()
        {
            var userId = GetUserId();
            var whiteList = await _whiteListService.GetWhiteLisTAsync(userId);

            return Ok(whiteList);
        }

        [HttpPost("AddToWhiteList")]
        public async Task<ActionResult<WhiteListDto>> AddToWhiteList(AddToWhiteListDto addToWhiteListDto)
        {
            var userId = GetUserId();
            var whiteList = await _whiteListService.AddToWhiteListAsync(userId, addToWhiteListDto);

            return Ok(whiteList);
        }

        [HttpGet("check/{bookId}")]
        public async Task<ActionResult<bool>> CheckInWhiteList(Guid bookId)
        {
            var userId = GetUserId();
            var isInWhiteList = await _whiteListService.IsInWhiteListAsync(userId, bookId);

            return Ok(isInWhiteList);
        }

        [HttpDelete("remove/{bookId}")]
        public async Task<ActionResult<WhiteListDto>> RemoveFromWhiteList(Guid bookId)
        {
            var userId = GetUserId();
            var whiteList = await _whiteListService.RemoveFromWhiteListAsync(userId, bookId);

            return Ok(whiteList);
        }
    }
}
