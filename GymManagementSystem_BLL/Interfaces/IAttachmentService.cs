using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymManagementSystem_BLL.Interfaces
{
        public interface IAttachmentService
        {
            Task<string?> UploadAsync(string folderName, IFormFile file);
            bool Delete(string folderName, string fileName);
        }
}
