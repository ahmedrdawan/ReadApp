using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.DTOs.User.Request;
using MyReadsApp.Core.Services.Interfaces;

namespace MyReadsApp.API.Controllers
{
    /// <summary>
    /// Handles user profile management endpoints including editing user information and managing avatars.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        /// <summary>
        /// Updates user profile information.
        /// </summary>
        /// <param name="request">User profile update data.</param>
        /// <returns>
        /// HTTP response indicating success or failure of profile update.
        /// </returns>
        [HttpPut("edit-information")]
        public async Task<IActionResult> EditInformationUser([FromBody] EditInformationUserRequest request)
        {
            var result = await _userServices.EditInformationUser(request);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Removes the user's avatar or profile image.
        /// </summary>
        /// <returns>
        /// HTTP response indicating success or failure of avatar removal.
        /// </returns>
        [HttpDelete("remove-avatar")]
        public async Task<IActionResult> RemoveAvatarOrImage()
        {
            var result = await _userServices.RemoveAvatarOrImage();
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Uploads or updates the user's avatar or profile image.
        /// </summary>
        /// <param name="request">File storage request containing the image to upload.</param>
        /// <returns>
        /// HTTP response indicating success or failure of image upload.
        /// </returns>
        [HttpPut("upload-avatar")]
        public async Task<IActionResult> UploadImageOrAvata([FromForm] FileStorageRequest request)
        {
            var result = await _userServices.UploadImageOrAvata(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}
