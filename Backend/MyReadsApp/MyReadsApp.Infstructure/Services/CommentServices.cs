using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.DTOs.Comment.Request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;
using Microsoft.EntityFrameworkCore;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Implements comment management in the infrastructure layer: create, update, delete, and fetch comments.
    /// Coordinates with repositories and caching to provide consistent responses.
    /// </summary>
    public class CommentServices : ICommentServises
    {
        private readonly IGenericRepository<Comment> _repository;
        private readonly IUserAuthServices _userAuthServices;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public CommentServices(IGenericRepository<Comment> repository, AppDbContext context, IUserAuthServices userAuthServices, IDistributedCache cache)
        {
            _repository = repository;
            _context = context;
            _userAuthServices = userAuthServices;
            _cache = cache;
        }

        /// <summary>
        /// Creates a new comment on a post after validating user and post exist.
        /// </summary>
        /// <param name="request">Create comment request DTO.</param>
        /// <returns>A Response containing the created comment response.</returns>
        public async Task<Response<CommentResponse>> CreateAsync(CreateCommentRequest request)
        {
            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
                return Response<CommentResponse>.Failure("The User Not Found", 404);

            var post = await _context.Posts.FindAsync(request.PostId);
            if (post == null)
                return Response<CommentResponse>.Failure("The Post Not Found", 404);

            var entity = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                PostId = request.PostId,
                content = request.content,
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);

            return Response<CommentResponse>.Success(response);
        }

        /// <summary>
        /// Deletes a comment by its identifier after verifying ownership.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to delete.</param>
        /// <returns>A Response containing the deleted comment response.</returns>
        public async Task<Response<CommentResponse>> DeleteAsync(Guid commentId)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return Response<CommentResponse>.Failure("The Comment Not Found", 404);

            if (comment.UserId != _userAuthServices.GetCurrentUser())
                return Response<CommentResponse>.Failure("The User Not Authorized", 403);

            await _repository.DeleteAsync(comment);
            await _cache.RemoveAsync(GetCacheKey(commentId));

            return Response<CommentResponse>.Success(BuildResponse(comment));
        }

        /// <summary>
        /// Updates an existing comment after verifying ownership.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment to update.</param>
        /// <param name="request">Update comment request DTO.</param>
        /// <returns>A Response containing the updated comment response.</returns>
        public async Task<Response<CommentResponse>> UpdateAsync(Guid commentId, UpdateCommentRequest request)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return Response<CommentResponse>.Failure("The Comment Not Found", 404);

            if (comment.UserId != _userAuthServices.GetCurrentUser())
                return Response<CommentResponse>.Failure("The User Not Authorized", 403);

            comment.content = request.content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(comment);
            var response = BuildResponse(comment);
            await _cache.SetRecordAsync(GetCacheKey(commentId), response);

            return Response<CommentResponse>.Success(response);
        }

        /// <summary>
        /// Retrieves a comment by its identifier, using cache when available.
        /// </summary>
        /// <param name="commentId">The unique identifier of the comment.</param>
        /// <returns>A Response containing the comment response.</returns>
        public async Task<Response<CommentResponse>> GetAsync(Guid commentId)
        {
            var cached = await _cache.GetRecordAsync<CommentResponse>(GetCacheKey(commentId));
            if (cached is not null)
                return Response<CommentResponse>.Success(cached);

            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return Response<CommentResponse>.Failure("Comment not found", 404);

            var response = BuildResponse(comment);
            await _cache.SetRecordAsync(GetCacheKey(commentId), response);
            return Response<CommentResponse>.Success(response);
        }

        /// <summary>
        /// Retrieves a paginated list of comments for a specific post.
        /// </summary>
        /// <param name="postId">The post identifier for which to retrieve comments.</param>
        /// <param name="pageNumber">Page number for pagination (default: 1).</param>
        /// <param name="pageSize">Number of items per page (default: 10).</param>
        /// <returns>A Response containing a paged result of CommentResponse.</returns>
        public async Task<Response<MyReadsApp.Core.Common.PagedResult<CommentResponse>>> GetListAsync(Guid postId, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var q = _context.Comments.Where(c => c.PostId == postId).OrderByDescending(c => c.CreatedAt);
            var total = await q.LongCountAsync();
            var items = await q.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var list = items.Select(BuildResponse).ToList();
            var paged = new MyReadsApp.Core.Common.PagedResult<CommentResponse>
            {
                Items = list,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };

            return Response<MyReadsApp.Core.Common.PagedResult<CommentResponse>>.Success(paged);
        }

        private static string GetCacheKey(Guid id) => $"Comment:{id}";

        private static CommentResponse BuildResponse(Comment comment)
        {
            return new CommentResponse
            {
                Id = comment.Id,
                content = comment.content,
                CreatedAt = comment.CreatedAt,
                UserId = comment.UserId,
                PostId = comment.PostId,
            };
        }
    }
}
