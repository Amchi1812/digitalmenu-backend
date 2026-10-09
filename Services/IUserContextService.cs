namespace DigitalMenu.Services;

public interface IUserContextService
{
    Guid GetRestaurantId();
    string GetUserEmail();
    string GetUserRole();
}