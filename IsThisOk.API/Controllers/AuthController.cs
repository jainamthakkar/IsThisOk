// IsThisOkay.API/Controllers/AuthController.cs
using IsThisOk.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IsThisOkay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        // 📝 Sign Up Endpoint
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            try
            {
                // 🔍 Basic validation
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

                if (request.Password.Length < 6)
                {
                    return BadRequest(new { message = "Password must be at least 6 characters" });
                }

                if (!IsValidEmail(request.Email))
                {
                    return BadRequest(new { message = "Invalid email format" });
                }

                // 🚀 Call UserService to create account
                var result = await _userService.SignUpAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation($"New user registered with anonymous name: {result.AnonymousDisplayName}");

                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        token = result.Token,
                        anonymousName = result.AnonymousDisplayName
                    });
                }
                else
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "Registration failed. Please try again." });
            }
        }

        // 🔑 Sign In Endpoint  
        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
        {
            try
            {
                // 🔍 Basic validation
                if (string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    return BadRequest(new { message = "Email and password are required" });
                }

                // 🚀 Call UserService to authenticate
                var result = await _userService.SignInAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation($"User signed in with anonymous name: {result.AnonymousDisplayName}");

                    return Ok(new
                    {
                        success = true,
                        message = result.Message,
                        token = result.Token,
                        anonymousName = result.AnonymousDisplayName
                    });
                }
                else
                {
                    return Unauthorized(new { success = false, message = result.Message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user sign in");
                return StatusCode(500, new { message = "Sign in failed. Please try again." });
            }
        }

        // 🔍 Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}