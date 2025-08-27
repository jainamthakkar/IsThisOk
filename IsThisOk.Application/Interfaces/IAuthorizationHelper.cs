namespace IsThisOk.Application.Interfaces
{
    public interface IAuthorizationHelper
    {
        string GetCurrentUserId(System.Security.Claims.ClaimsPrincipal user);
        string GetCurrentUserRole(System.Security.Claims.ClaimsPrincipal user);
        bool IsCurrentUser(System.Security.Claims.ClaimsPrincipal user, string userId);
        bool IsAdmin(System.Security.Claims.ClaimsPrincipal user);
    }
}