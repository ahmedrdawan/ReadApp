using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.Services.Interfaces;
using System.Threading.Tasks;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles file storage operations including uploading and deleting files.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class FileController : ControllerBase
    {
        private readonly IFileStorage _fileStorage;

        public FileController(IFileStorage fileStorage)
        {
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Uploads a file to storage.
        /// </summary>
        /// <param name="request">File storage request containing the file to upload.</param>
        /// <returns>
        /// HTTP response indicating success or failure of file upload.
        /// </returns>
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileStorageRequest request)
        {
            var result = await _fileStorage.UploadAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deletes a file from storage by its filename.
        /// </summary>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>
        /// HTTP response indicating success or failure of file deletion.
        /// </returns>
        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
           var result = await _fileStorage.DeleteAsync(fileName);
            return result.IsSuccess ? Ok("File deleted successfully.") : BadRequest(result);
        }
    }
}