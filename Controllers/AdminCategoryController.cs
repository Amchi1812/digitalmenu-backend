using DigitalMenu.DTOs;
using DigitalMenu.Entities;
using DigitalMenu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/admin/categories")]
[Authorize]
public class AdminCategoryController: ControllerBase
{
    private readonly ICategoryService  _categoryService;

    public AdminCategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    

    [HttpGet]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _categoryService.GetAllForCurrentRestaurantAsync();
        
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (result == null) return NotFound("Kategorija nije pronađena.");
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto category)
    {
        var result = await _categoryService.CreateAsync(category);
        
        
        
        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Id }, result);
        
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto category)
    {
        var result = await _categoryService.UpdateAsync(id, category);
        if (result == false) return NotFound("Kategorija nije pronađena.");
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        
        if (result == false) return NotFound("Kategorija nije pronađena.");
        return NoContent();
    }
    
    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] ReorderCategoryDto[] dtos)
    {
        var success = await _categoryService.ReorderCategoryAsync(dtos);

        if (!success)
        {
            return BadRequest("Neuspješno ažuriranje redoslijeda kategorija.");
        }

        return NoContent(); 
    }
    
    
    

    
}