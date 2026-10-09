using DigitalMenu.DTOs;
using DigitalMenu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;


[ApiController]
[Route("api/admin/items")]
[Authorize]

public class AdminMenuItemController: ControllerBase
{
    private readonly IMenuItemService _menuItemService;
    
    public AdminMenuItemController(IMenuItemService menuItemService)
    {
        
        _menuItemService = menuItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems(Guid? categoryId = null)
    {
        var result = await _menuItemService.GetAllForCurrentRestaurantAsync(categoryId);
        
        
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItemById(Guid id){
        var result = await _menuItemService.GetByIdAsync(id);

        if (result == null)
        {
            return NotFound("Artikal nije pronađen.");
        }
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateItem([FromBody] CreateMenuItemDto dto)
    {
        try
        {
            var result = await _menuItemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetItemById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); 
        }
    }

   

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] UpdateMenuItemDto dto)
    {
        try
        {
            var success = await _menuItemService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound("Artikal nije pronađen.");
            }

            return NoContent();

        }
        catch (ArgumentException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(Guid id)
    {
        var result = await _menuItemService.DeleteAsync(id);

        if (result == false)
        {
            return NotFound("Artikal nije pronađen.");
        }
        
        return NoContent();
    }
}