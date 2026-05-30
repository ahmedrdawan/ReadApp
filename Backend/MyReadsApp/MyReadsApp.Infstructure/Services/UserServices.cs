using Microsoft.AspNetCore.Identity;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.DTOs.User.Request;
using MyReadsApp.Core.DTOs.User.Response;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Manages user-related operations in the infrastructure layer, including profile updates,
    /// retrieval, and coordination with identity/auth systems and caches.
    /// </summary>
    public class UserServices : IUserServices
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserAuthServices _userAuthServices;
        private readonly IFileStorage _fileStorage;

        public UserServices(
            UserManager<User> userManager,
            IUserAuthServices userAuthServices,
            IFileStorage fileStorage)
        {
            _userManager = userManager;
            _userAuthServices = userAuthServices;
            _fileStorage = fileStorage;
        }

        /// <summary>
        /// Updates the current user's profile information including username, country, gender, and birth date.
        /// </summary>
        /// <param name="request">Edit information user request DTO with updated profile data.</param>
        /// <returns>A Response containing the updated user profile response.</returns>
        public async Task<Response<UserProfileResponse>> EditInformationUser(EditInformationUserRequest request)
        {
            var userId = _userAuthServices.GetCurrentUser();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<UserProfileResponse>.Failure("User not found.", 404);

            if (!string.IsNullOrWhiteSpace(request.UserName) && !string.Equals(request.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
            {
                var userByName = await _userManager.FindByNameAsync(request.UserName);
                if (userByName != null && userByName.Id != user.Id)
                    return Response<UserProfileResponse>.Failure("User name already exists.", 409);

                user.UserName = request.UserName;
            }

            user.Country = request.Country;
            user.Gender = request.Gender;
            user.BirthDate = request.BirthDate;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Response<UserProfileResponse>.Failure(result.Errors.Select(e => e.Description).FirstOrDefault() ?? "User update failed.", 400);

            return Response<UserProfileResponse>.Success(BuildUserProfileResponse(user));
        }

        /// <summary>
        /// Uploads a new user avatar or profile image, replacing any existing image.
        /// </summary>
        /// <param name="request">File storage request containing the image file to upload.</param>
        /// <returns>A Response containing the updated user profile response.</returns>
        public async Task<Response<UserProfileResponse>> UploadImageOrAvata(FileStorageRequest request)
        {
            var userId = _userAuthServices.GetCurrentUser();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<UserProfileResponse>.Failure("User not found.", 404);

            var uploadedFile = await _fileStorage.UploadAsync(request);
            if (!uploadedFile.IsSuccess || string.IsNullOrWhiteSpace(uploadedFile.Value))
                return Response<UserProfileResponse>.Failure(uploadedFile.Message ?? "File upload failed.", uploadedFile.StatusCode);

            var oldImage = user.UserImage;
            user.UserImage = uploadedFile.Value;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await _fileStorage.DeleteAsync(uploadedFile.Value);
                return Response<UserProfileResponse>.Failure(updateResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "User update failed.", 400);
            }

            if (!string.IsNullOrWhiteSpace(oldImage))
                await _fileStorage.DeleteAsync(oldImage);

            return Response<UserProfileResponse>.Success(BuildUserProfileResponse(user));
        }


        /// <summary>
        /// Removes the current user's avatar or profile image.
        /// </summary>
        /// <returns>A Response containing the updated user profile response with null UserImage.</returns>
        public async Task<Response<UserProfileResponse>> RemoveAvatarOrImage()
        {
            var userId = _userAuthServices.GetCurrentUser();
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return Response<UserProfileResponse>.Failure("User not found.", 404);

            if (string.IsNullOrWhiteSpace(user.UserImage))
                return Response<UserProfileResponse>.Failure("User does not have an avatar image.", 400);

            var currentImage = user.UserImage!;
            user.UserImage = null;

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                user.UserImage = currentImage;
                return Response<UserProfileResponse>.Failure(updateResult.Errors.Select(e => e.Description).FirstOrDefault() ?? "User update failed.", 400);
            }

            await _fileStorage.DeleteAsync(currentImage);

            return Response<UserProfileResponse>.Success(BuildUserProfileResponse(user));
        }

        private static UserProfileResponse BuildUserProfileResponse(User user)
        {
            return new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Country = user.Country,
                Gender = user.Gender,
                BirthDate = user.BirthDate,
                UserImage = user.UserImage
            };
        }
    }
}
