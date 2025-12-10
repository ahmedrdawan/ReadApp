using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IPostServices
    {
        Task<Response<PostResponse>> CreateAsync(Post post);
        Task<Response<PostResponse>> DeleteAsync(Guid PostId);
        Task<Response<PostResponse>> UpdateAsync(Guid PostId, Post newPost);
        Task<Response<PostResponse>> GetAsync(Guid PostId);
    }
}
