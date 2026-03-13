using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
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

        public async Task<Response<CommentResponse>> CreateAsync(Comment entity)
        {
            var user = await _context.Users.FindAsync(entity.UserId);
            if (user == null)
                return Response<CommentResponse>.Failure("The User Not Found", 404);

            var post = await _context.Posts.FindAsync(entity.PostId);
            if (post == null)
                return Response<CommentResponse>.Failure("The Post Not Found", 404);

            await _repository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);

            return Response<CommentResponse>.Success(response);
        }

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

        public async Task<Response<CommentResponse>> UpdateAsync(Guid commentId, Comment newEntity)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment == null)
                return Response<CommentResponse>.Failure("The Comment Not Found", 404);

            if (comment.UserId != _userAuthServices.GetCurrentUser())
                return Response<CommentResponse>.Failure("The User Not Authorized", 403);

            comment.content = newEntity.content;
            comment.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(comment);
            var response = BuildResponse(comment);
            await _cache.SetRecordAsync(GetCacheKey(commentId), response);

            return Response<CommentResponse>.Success(response);
        }

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
