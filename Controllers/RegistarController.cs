using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/[controller]")]

public class RegistarController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public  RegistarController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> AddUserProduction([FromBody] CreateRestaurantAdminDto dto)
    {
        var newUser = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            RestaurantId = null,
            Role = "SuperAdmin"
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(newUser);
    }
}