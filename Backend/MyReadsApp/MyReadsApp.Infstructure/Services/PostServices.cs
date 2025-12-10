using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.Post.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Core.Common;
using Microsoft.AspNetCore.Http;

namespace MyReadsApp.Infstructure.Services
{
    public class PostServices : IPostServices
    {
        private readonly IGenericRepository<Post> _genericRepository;
        private readonly IUserAuthServices _userAuthServices;
        private readonly AppDbContext _context;

        public PostServices(IGenericRepository<Post> genericRepository, AppDbContext context, IUserAuthServices userAuthServices)
        {
            _genericRepository = genericRepository;
            _context = context;
            _userAuthServices = userAuthServices;
        }

        public async Task<Response<PostResponse>> CreateAsync(Post entity)
        {
            var book = await _context.Books.FindAsync(entity.BookId);
            var user = await _context.Users.FindAsync(entity.UserId);

            if (book is null || user is null)
                return Response<PostResponse>.Failure("The Book Or User Not Found", 404);

            await _genericRepository.CreateAsync(entity);
            PostResponse response = BuildResponse(entity);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> DeleteAsync(Guid PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post is null)
                return Response<PostResponse>.Failure("The Post Not Found",404);

            if (post.UserId != _userAuthServices.GetCurrentUser())
                return Response<PostResponse>.Failure("The User Not Authorize", 403);

            await _genericRepository.DeleteAsync(PostId);
            PostResponse response = BuildResponse(post);
            return Response<PostResponse>.Success(response);
        }

        public async Task<Response<PostResponse>> GetAsync(Guid PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post == null)
                return Response<PostResponse>.Failure("The Post Not Found", 404);
            PostResponse response = BuildResponse(post);
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

            await _genericRepository.UpdateAsync(PostId, post);
            PostResponse response = BuildResponse(post);
            return Response<PostResponse>.Success(response);
        }

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
