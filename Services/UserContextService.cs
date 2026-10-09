

namespace DigitalMenu.Services;
using System.Security.Claims;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetRestaurantId()
    {
        var restaurantIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("restaurant_id")?.Value;

        if (string.IsNullOrEmpty(restaurantIdClaim) || !Guid.TryParse(restaurantIdClaim, out var restaurantId))
        {
            throw new UnauthorizedAccessException("Korisnik nije autorizovan ili nedostaje restaurant_id u tokenu.");
        }

        return restaurantId;
    }

    public string GetUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;
        
    }

    public string GetUserRole()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }
}

