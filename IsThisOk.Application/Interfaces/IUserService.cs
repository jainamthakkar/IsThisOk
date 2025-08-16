using IsThisOk.Domain.Entities;

namespace IsThisOk.Application.Interfaces
{
    public class SignUpRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Gender { get; set; } = null!;
    }

    public class SignInRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string AnonymousDisplayName { get; set; } = null!;
        public string Message { get; set; } = null!;
        public bool Success { get; set; }
    }

    public interface IUserService
    {
        Task<AuthResponse> SignUpAsync(SignUpRequest request);
        Task<AuthResponse> SignInAsync(SignInRequest request);
        Task<UserMst?> GetUserByIdAsync(string userId);
    }
}