using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using MyReadsApp.Infstructure.Services.Cache;

namespace MyReadsApp.Infstructure.Services
{
    public class PostServices : IPostServices
    {
        private readonly IGenericRepository<Post> _genericRepository;
        private readonly IUserAuthServices _userAuthServices;
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public PostServices(IGenericRepository<Post> genericRepository, AppDbContext context, IUserAuthServices userAuthServices, IDistributedCache cache)
        {
            _genericRepository = genericRepository;
            _context = context;
            _userAuthServices = userAuthServices;
            _cache = cache;
        }

        public async Task<Response<PostResponse>> CreateAsync(Post entity)
        {
            var book = await _context.Books.FindAsync(entity.BookId);
            var user = await _context.Users.FindAsync(entity.UserId);

            if (book is null || user is null)
                return Response<PostResponse>.Failure("The Book Or User Not Found", 404);

            await _genericRepository.CreateAsync(entity);
            var response = BuildResponse(entity);
            await _cache.SetRecordAsync(GetCacheKey(entity.Id), response);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> DeleteAsync(Guid PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post is null)
                return Response<PostResponse>.Failure("The Post Not Found",404);

            if (post.UserId != _userAuthServices.GetCurrentUser())
                return Response<PostResponse>.Failure("The User Not Authorize", 403);

            await _genericRepository.DeleteAsync(post);
            await _cache.RemoveAsync(GetCacheKey(PostId));
            return Response<PostResponse>.Success(BuildResponse(post));
        }

        public async Task<Response<PostResponse>> GetAsync(Guid PostId)
        {
            var cached = await _cache.GetRecordAsync<PostResponse>(GetCacheKey(PostId));
            if (cached is not null)
                return Response<PostResponse>.Success(cached);

            var post = await _context.Posts.FindAsync(PostId);
            if (post == null)
                return Response<PostResponse>.Failure("The Post Not Found", 404);

            var response = BuildResponse(post);
            await _cache.SetRecordAsync(GetCacheKey(PostId), response);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> UpdateAsync(Guid PostId, Post NewEntity)
        {
            var post = await _context.Posts.FindAsync(PostId);
            var book = await _context.Books.FindAsync(NewEntity.BookId);
            if (post is null || book is null)
                return Response<PostResponse>.Failure($"Post or Book not found.", 404);

            if (post.UserId != _userAuthServices.GetCurrentUser())
                return Response<PostResponse>.Failure("The User Not Authorize", 403);


            post.BookId = NewEntity.BookId;
            post.UserId = NewEntity.UserId;
            post.UpdatedAt = NewEntity.UpdatedAt;

            await _genericRepository.UpdateAsync(post);
            var response = BuildResponse(post);
            await _cache.SetRecordAsync(GetCacheKey(PostId), response);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<MyReadsApp.Core.Common.PagedResult<MyReadsApp.Core.DTOs.Post.Response.PostFeedItem>>> GetFeedAsync(int pageNumber = 1, int pageSize = 10, Guid? currentUserId = null)
        {
            if (pageNumber <= 0) pageNumber = 1;
            if (pageSize <= 0) pageSize = 10;

            var query = _context.Posts.Include(p => p.Book).Include(p => p.User).AsQueryable();
            var total = await query.LongCountAsync();

            var posts = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = new List<MyReadsApp.Core.DTOs.Post.Response.PostFeedItem>();
            var postIds = posts.Select(p => p.Id).ToList();

            var likes = await _context.Likes.Where(l => postIds.Contains(l.PostId)).ToListAsync();
            var comments = await _context.Comments.Where(c => postIds.Contains(c.PostId)).ToListAsync();
            var favorites = await _context.FaviorateBooks.Where(f => postIds.Contains(f.BookId)).ToListAsync();

            foreach (var p in posts)
            {
                var likeCount = likes.Count(l => l.PostId == p.Id);
                var commentCount = comments.Count(c => c.PostId == p.Id);
                var isLiked = currentUserId.HasValue && likes.Any(l => l.PostId == p.Id && l.UserId == currentUserId.Value);
                var isFavorited = currentUserId.HasValue && favorites.Any(f => f.BookId == p.BookId && f.UserId == currentUserId.Value);

                var userResp = new MyReadsApp.Core.DTOs.User.Response.UserProfileResponse
                {
                    Id = p.User.Id,
                    UserName = p.User.UserName,
                    Email = p.User.Email,
                    Country = p.User.Country,
                    Gender = p.User.Gender,
                    BirthDate = p.User.BirthDate,
                    UserImage = p.User.UserImage
                };

                var bookResp = new MyReadsApp.Core.DTOs.Book.Response.BookAuthorResponse
                {
                    Id = p.Book.Id,
                    Title = p.Book.Title,
                    BookImage = p.Book.BookImage,
                    Description = p.Book.Description,
                    AuthorId = p.Book.AuthorId,
                    AuthorName = p.Book.Author?.AuthorName
                };

                items.Add(new MyReadsApp.Core.DTOs.Post.Response.PostFeedItem
                {
                    Id = p.Id,
                    User = userResp,
                    Book = bookResp,
                    CreatedAt = p.CreatedAt,
                    Action = null,
                    Rating = null,
                    LikeCount = likeCount,
                    CommentCount = commentCount,
                    IsLikedByCurrentUser = isLiked,
                    IsFavoritedByCurrentUser = isFavorited
                });
            }

            var paged = new MyReadsApp.Core.Common.PagedResult<MyReadsApp.Core.DTOs.Post.Response.PostFeedItem>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total
            };

            return Response<MyReadsApp.Core.Common.PagedResult<MyReadsApp.Core.DTOs.Post.Response.PostFeedItem>>.Success(paged);
        }

        private static string GetCacheKey(Guid id) => $"Post:{id}";

        private static PostResponse BuildResponse(Post entity)
        {
            return new PostResponse
            {
                Id = entity.Id,
                BookId = entity.BookId,
                UserId = entity.UserId,
                CreatedAt = entity.CreatedAt,
            };
        }
    }
}
