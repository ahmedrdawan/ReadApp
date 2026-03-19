using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FileStorage;
using MyReadsApp.Core.DTOs.User.Request;
using MyReadsApp.Core.DTOs.User.Response;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IUserServices
    {
        Task<Response<UserProfileResponse>> EditInformationUser(EditInformationUserRequest request);
        Task<Response<UserProfileResponse>> UploadImageOrAvata(FileStorageRequest request);
        Task<Response<UserProfileResponse>> RemoveAvatarOrImage();
    }
}
