using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.Services.Interfaces;
using System.Threading.Tasks;

namespace MyReadsApp.API.Controllers
{
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

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] FileStorageRequest request)
        {
            var result = await _fileStorage.UploadAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> Delete(string fileName)
        {
           var result = await _fileStorage.DeleteAsync(fileName);
            return result.IsSuccess ? Ok("File deleted successfully.") : BadRequest(result);
        }
    }
}