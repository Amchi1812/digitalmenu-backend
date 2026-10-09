using DigitalMenu.Services;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetPublicMenu(string slug)
    {
        var menu = await _menuService.GetPublicMenuBySlugAsync(slug);

        if (menu is null)
        {
            return NotFound(new { message = "Meni nije pronađen ili je restoran neaktivan." });
        }

        return Ok(menu);
    }
}