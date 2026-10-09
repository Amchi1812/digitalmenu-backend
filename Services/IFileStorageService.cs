namespace DigitalMenu.Services;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName);
    void DeleteFile(string fileUrl);
}