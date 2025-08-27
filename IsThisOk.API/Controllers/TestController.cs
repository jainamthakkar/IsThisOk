using IsThisOk.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IsThisOkay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IAuthorizationHelper _authHelper;

        public TestController(IAuthorizationHelper authHelper)
        {
            _authHelper = authHelper;
        }

        // Anyone can access this
        [HttpGet("public")]
        public IActionResult PublicEndpoint()
        {
            return Ok(new { message = "This is public - no login needed!" });
        }

        // Must be logged in
        [HttpGet("protected")]
        [Authorize(Policy = "UserPolicy")]
        public IActionResult ProtectedEndpoint()
        {
            var userId = _authHelper.GetCurrentUserId(User);
            var role = _authHelper.GetCurrentUserRole(User);

            return Ok(new
            {
                message = "You are authenticated!",
                userId = userId,
                role = role,
                note = "This proves your JWT token works!"
            });
        }

        // Only admins can access
        [HttpGet("admin-only")]
        [Authorize(Policy = "AdminPolicy")]
        public IActionResult AdminOnlyEndpoint()
        {
            return Ok(new { message = "Welcome Admin! You have special powers!" });
        }

        // Check if user owns something
        [HttpGet("check-ownership/{userId}")]
        [Authorize(Policy = "PostOwnerPolicy")]
        public IActionResult CheckOwnership(string userId)
        {
            var isOwner = _authHelper.IsCurrentUser(User, userId);

            return Ok(new
            {
                message = $"Are you the owner of userId {userId}?",
                isOwner = isOwner,
                yourUserId = _authHelper.GetCurrentUserId(User)
            });
        }
    }
}