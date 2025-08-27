using IsThisOk.Application.Interfaces;
using System.Security.Claims;

namespace IsThisOk.Application.Services
{
    public class AuthorizationHelper : IAuthorizationHelper
    {
        public string GetCurrentUserId(ClaimsPrincipal user)
        {
            // Extract userId from JWT token
            return user.FindFirst("userId")?.Value ?? string.Empty;
        }

        public string GetCurrentUserRole(ClaimsPrincipal user)
        {
            // Extract role from JWT token
            return user.FindFirst("role")?.Value ?? string.Empty;
        }

        public bool IsCurrentUser(ClaimsPrincipal user, string userId)
        {
            // Check if the current user is the same as the provided userId
            var currentUserId = GetCurrentUserId(user);
            return currentUserId == userId;
        }

        public bool IsAdmin(ClaimsPrincipal user)
        {
            // Check if current user is admin
            var role = GetCurrentUserRole(user);
            return role == "Admin";
        }

        public bool IsSuperAdmin(ClaimsPrincipal user)
        {
            var role = GetCurrentUserRole(user);
            return role == "SuperAdmin";
        }

        public bool IsAdminOrHigher(ClaimsPrincipal user)
        {
            var role = GetCurrentUserRole(user);
            return role == "Admin" || role == "SuperAdmin";
        }
    }
}