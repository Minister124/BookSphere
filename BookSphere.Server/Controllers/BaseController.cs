using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BookSphere.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if(userIdClaim == null)
            {
                throw new UnauthorizedAccessException("User Id not found in token");
            }
            return Guid.Parse(userIdClaim.Value);
        }
    }
}
