using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.DTOs.User.Request;
using MyReadsApp.Core.Services.Interfaces;

namespace MyReadsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPut("edit-information")]
        public async Task<IActionResult> EditInformationUser([FromBody] EditInformationUserRequest request)
        {
            var result = await _userServices.EditInformationUser(request);
            return StatusCode(result.StatusCode, result);
        }


        [HttpDelete("remove-avatar")]
        public async Task<IActionResult> RemoveAvatarOrImage()
        {
            var result = await _userServices.RemoveAvatarOrImage();
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut("upload-avatar")]
        public async Task<IActionResult> UploadImageOrAvata([FromForm] FileStorageRequest request)
        {
            var result = await _userServices.UploadImageOrAvata(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
