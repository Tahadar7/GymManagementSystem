using GymManagementSystem_BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;

namespace GymManagementSystem_BLL.Services
{
    public class AttachmentService(IWebHostEnvironment webHostEnvironment) : IAttachmentService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long _maximumFileSize = 5 * 1024 * 1024; // 5MB

        public async Task<string?> UploadAsync(string folderName, IFormFile file)
        {
            try
            {
                if (string.IsNullOrEmpty(folderName) || file is null || file.Length == 0) 
                { 
                    return null; 
                }
                if (file.Length > _maximumFileSize)  
                {
                    return null;
                }

                var extension = Path.GetExtension(file.FileName).ToLower();
                if (!_allowedExtensions.Contains(extension))  // not allowed extension file
                {
                    return null;
                }

                var folderPath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName);
              
                if (!Directory.Exists(folderPath))  // Check if the folder exists, if not create it
                { 
                    Directory.CreateDirectory(folderPath);
                }

                var fileName = Guid.NewGuid().ToString() + extension;
                var filePath = Path.Combine(folderPath, fileName);

                using var fileStream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(fileStream);

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload file to folder '{folderName}': {ex}");
                return null;
            }
        }
        public bool Delete(string folderName, string fileName)
        {
            try
            {
                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName))
                { 
                    return false; 
                }

                var filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", folderName, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete file '{fileName}': {ex}");
                return false;
            }
        }
    }
}