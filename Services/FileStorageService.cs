using Imagekit;
using Imagekit.Models.Files;
using DigitalMenu.Entities;
using Microsoft.Extensions.Configuration;

namespace DigitalMenu.Services;

public class FileStorageService : IFileStorageService
{
    private readonly ImageKitClient _imageKitClient;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB

    public FileStorageService(IConfiguration configuration)
    {
        var urlEndpoint = configuration["ImageKitSettings:UrlEndpoint"];
        var publicKey = configuration["ImageKitSettings:PublicKey"];
        var privateKey = configuration["ImageKitSettings:PrivateKey"];

        if (string.IsNullOrEmpty(urlEndpoint) || string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
        {
            throw new ArgumentException("ImageKit postavke nisu ispravno konfigurisane u appsettings.json.");
        }

        
        _imageKitClient = new ImageKitClient
        {
            PrivateKey = privateKey
        };
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderName)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Datoteka nije proslijeđena ili je prazna.");
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            throw new ArgumentException("Veličina datoteke ne smije prelaziti 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            throw new ArgumentException("Nedozvoljen format slike. Dozvoljeni su samo .jpg, .jpeg, .png i .webp.");
        }

        
        using var stream = file.OpenReadStream();

        
        var uploadParams = new FileUploadParams
        {
            File = stream,
            FileName = $"{Guid.NewGuid()}{extension}",
            Folder = $"/digitalmenu/{folderName}/",
            UseUniqueFileName = true
        };

        
        FileUploadResponse response = await _imageKitClient.Files.Upload(uploadParams);

        if (response == null || string.IsNullOrEmpty(response.Url))
        {
            throw new Exception("Greška pri slanju slike na ImageKit API.");
        }

       
        return response.Url;
    }

    public void DeleteFile(string fileUrl)
    {
        if (string.IsNullOrEmpty(fileUrl)) return;

        try
        {
            var uri = new Uri(fileUrl);
            var fileName = Path.GetFileName(uri.AbsolutePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Greška pri brisanju slike sa ImageKit-a: {ex.Message}");
        }
    }
}