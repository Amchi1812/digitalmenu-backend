using DigitalMenu.DTOs;

namespace DigitalMenu.Services;

public interface IMenuItemService
{
    Task<List<MenuItemDto>> GetAllForCurrentRestaurantAsync(Guid? categoryId = null);
    Task<MenuItemDto?> GetByIdAsync(Guid id);
    Task<MenuItemDto> CreateAsync(CreateMenuItemDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateMenuItemDto dto);
    Task<bool> DeleteAsync(Guid id);
}