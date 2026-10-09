using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalMenu.Services;

public class MenuService : IMenuService
{
    private readonly ApplicationDbContext _context;

    public MenuService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PublicMenuResponseDto?> GetPublicMenuBySlugAsync(string slug)
    {
        var restaurant = await _context.Restaurants
            .AsNoTracking()
            .Where(r => r.Slug == slug && (r.IsActive ?? false))
            .Select(r => new PublicMenuResponseDto(
                r.Id,
                r.Name,
                r.Slug,
                r.IsActive ?? false,
                r.Categories
                    .Where(c => c.IsVisible ?? false)
                    .OrderBy(c => c.DisplayOrder)
                    .Select(c => new PublicCategoryDto(
                        c.Id,
                        c.Name,
                        c.DisplayOrder ?? 0,
                        c.MenuItemCategoryNavigations
                            .Where(i => i.IsAvailable ?? false)
                            .OrderBy(i => i.DisplayOrder)
                            .Select(i => new PublicMenuItemDto(
                                i.Id,
                                i.Name,
                                i.Description,
                                i.BasePrice,
                                i.ImageUrl,
                                i.IsAvailable ?? false,
                                i.DisplayOrder ?? 0,
                                i.ItemVariantMenuItems
                                    .Where(v => v.IsAvailable ?? false)
                                    .Select(v => new PublicItemVariantDto(
                                        v.Id,
                                        v.Name,
                                        v.Price,
                                        v.IsAvailable ?? false
                                    ))
                                    .ToList(),
                                i.MenuItemAllergenMenuItems
                                    .Select(a => a.Allergen.Name)
                                    .ToList()
                            ))
                            .ToList()
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync();

        return restaurant;
    }
}