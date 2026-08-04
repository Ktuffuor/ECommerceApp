using System.Security.Claims;
using Application.Interfaces;
using Application.Interfaces.Users;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    // Extracts the user's ID from either the standard NameIdentifier claim or the JWT 'sub' claim
    public Guid? UserId
    {
        get
        {
            var idString = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) 
                           ?? httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

            if (Guid.TryParse(idString, out var guid))
            {
                return guid;
            }

            return null;
        }
    }

    // Extracts the user's email address from the token claims
    public string? Email => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
}