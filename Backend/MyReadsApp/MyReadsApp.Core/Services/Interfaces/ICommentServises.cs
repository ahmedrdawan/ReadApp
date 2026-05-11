using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface ICommentServises
    {
        Task<Response<CommentResponse>> CreateAsync(Comment comment);
        Task<Response<CommentResponse>> DeleteAsync(Guid CommentId);
        Task<Response<CommentResponse>> UpdateAsync(Guid CommentId, Comment newComment);
        Task<Response<CommentResponse>> GetAsync(Guid CommentId);
        Task<Response<MyReadsApp.Core.Common.PagedResult<CommentResponse>>> GetListAsync(Guid postId, int pageNumber = 1, int pageSize = 10);
    }
}
