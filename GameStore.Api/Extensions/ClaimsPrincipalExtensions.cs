using System.Security.Claims;

namespace GameStore.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim is null)
        {
            throw new InvalidOperationException("User ID claim not found");
        }
        return int.Parse(userIdClaim.Value);
    }
}
