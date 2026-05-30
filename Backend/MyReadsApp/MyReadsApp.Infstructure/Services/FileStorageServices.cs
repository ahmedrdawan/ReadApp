
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Handles file storage operations (save, delete, retrieve) for the infrastructure layer.
    /// Abstracts storage provider details and returns accessible file information to upper layers.
    /// </summary>
    public class FileStorageServices : IFileStorage
    {
        private readonly long MaxFileSize = 2 * 1024 * 1024; // 2 MB
        private readonly string[] AllowedExtensions = new[] { "image/jpg", "image/jpeg", "image/png", "image/gif" };
        /// <summary>
        /// Uploads a file to the server after validating file size and type.
        /// </summary>
        /// <param name="request">File storage request containing the file to upload.</param>
        /// <returns>A Response containing the file path on successful upload.</returns>
        public async Task<Response<string>> UploadAsync(FileStorageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return Response<string>.Failure("No file uploaded.", 400);

            if (request.File.Length > MaxFileSize)
                return Response<string>.Failure("File size exceeds the 2 MB limit.", 400);

            if (!AllowedExtensions.Contains(request.File.ContentType))
                return Response<string>.Failure("Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.", 400);

            var UploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            if (!Directory.Exists(UploadsFolder))
                Directory.CreateDirectory(UploadsFolder);

            var filePath = Guid.NewGuid().ToString() + Path.GetExtension(request.File.FileName);

            using (var stream = new FileStream(Path.Combine(UploadsFolder, filePath), FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }
            return Response<string>.Success(filePath);
        }

        /// <summary>
        /// Deletes a file from the server by its file name.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A Response indicating success or failure of the deletion.</returns>
        public async Task<Response> DeleteAsync(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Response.Failure("File name is required.", 400);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", fileName);

            if (!File.Exists(fullPath))
                return Response.Failure("File does not exist.", 404);

            await Task.Run(() => File.Delete(fullPath));
            return Response.Success();
        }
    }
}
