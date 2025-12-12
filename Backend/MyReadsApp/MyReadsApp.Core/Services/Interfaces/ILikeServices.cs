using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Like.Response;
using MyReadsApp.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface ILikeServices
    {
        Task<Response<LikeResponse>> CreateAsync(Like like);
        Task<Response<LikeResponse>> DeleteAsync(Guid postId, Guid userId);
        Task<Response<int>> CountLikeAsync(Guid postId);
    }
}
