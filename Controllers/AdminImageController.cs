using DigitalMenu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DigitalMenu.Controllers;

[ApiController]
[Route("api/admin/image")]
[Authorize]
public class AdminImageController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    public AdminImageController(IFileStorageService fileStorageService)
    {
        _fileStorageService = fileStorageService;
    }
    
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file, [FromQuery] string folder = "items")
    {
        try
        {
            var imageUrl = await _fileStorageService.SaveFileAsync(file, folder);
            return Ok(new { url = imageUrl });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

