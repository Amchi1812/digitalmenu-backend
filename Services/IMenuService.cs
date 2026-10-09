using DigitalMenu.DTOs;

namespace DigitalMenu.Services
{
    public interface IMenuService
    {
        Task<PublicMenuResponseDto?> GetPublicMenuBySlugAsync(string slug);
    }
}

