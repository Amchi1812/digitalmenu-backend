using DigitalMenu.DTOs;

namespace DigitalMenu.Services;
public interface ICategoryService
{
    Task<List<CategoryDto>> GetAllForCurrentRestaurantAsync();
    Task<CategoryDto?> GetByIdAsync(Guid id);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateCategoryDto dto);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ReorderCategoryAsync(ReorderCategoryDto[] dtos);
}

