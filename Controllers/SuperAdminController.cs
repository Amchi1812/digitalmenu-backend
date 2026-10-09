using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using DigitalMenu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/superadmin")]
[Authorize(Roles = "SuperAdmin")]

public class SuperAdminController : ControllerBase
{
    private readonly ISuperAdminService _superAdminService;

    public SuperAdminController(ISuperAdminService superAdminService)
    {
        _superAdminService = superAdminService;
    }
    
    [HttpGet("restaurants")]
    public async Task<IActionResult> GetRestaurants()
    {
        var restaurants = await _superAdminService.GetAllRestaurantsAsync();
        return Ok(restaurants);
    }

    [HttpPost("restaurants")]
    
    public async Task<IActionResult> AddRestaurant([FromBody] CreateRestaurantDto dto)
    {
        try
        {
            var result = await _superAdminService.CreateRestaurantAsync(dto);
            return Ok(result);

        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] CreateRestaurantAdminDto dto)
    {
        try
        {
            var result = await _superAdminService.CreateRestaurantAdminAsync(dto);
            return Ok(new {message = "Korisnik uspješno kreiran."});

        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }
    
}