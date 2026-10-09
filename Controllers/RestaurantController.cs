using DigitalMenu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/restaurant")]
[Authorize]

public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _restaurantService;
    
    public RestaurantController(IRestaurantService restaurantService)
    {
        _restaurantService = restaurantService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var rsponse = await _restaurantService.GetByIdAsync(id);
        
        if (rsponse == null) return NotFound("Restoran ne postoji!");
        return Ok(rsponse);
    }
}