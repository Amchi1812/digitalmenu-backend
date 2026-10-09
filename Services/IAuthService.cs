using DigitalMenu.DTOs;

namespace DigitalMenu.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}

