using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrightMinds.Services.Helpers
{

        public static class PictureSettings
        {
            public static string UploadFile(IFormFile file, string FolderName)
            {
                var FolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", FolderName);
                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                var fileName = $"{Guid.NewGuid()}{file.FileName}";
                var filePath = Path.Combine(FolderPath, fileName);

                using var fs = new FileStream(filePath, FileMode.Create);
                file.CopyTo(fs);

                return Path.Combine("images", FolderName, fileName).Replace("\\", "/");
            }

            public static void DeleteFile(string PictureUrl)
            {
                if (string.IsNullOrEmpty(PictureUrl)) return;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", PictureUrl).Replace("\\", "/");
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

    }
