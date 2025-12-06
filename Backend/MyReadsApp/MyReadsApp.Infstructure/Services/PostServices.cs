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

namespace MyReadsApp.Infstructure.Services
{
    public class PostServices : IPostServices
    {
        private readonly IGenericRepository<Post> _genericRepository;
        private readonly AppDbContext _context;

        public PostServices(IGenericRepository<Post> genericRepository, AppDbContext context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<int> CreateAsync(Post entity)
        {
            var book = await _context.Books.FindAsync(entity.BookId);
            var user = await _context.Users.FindAsync(entity.UserId);

            if (book is null || user is null)
                throw new KeyNotFoundException("The Book Or User Not Found");

            return await _genericRepository.CreateAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid PostId)
        {
            var post = await _context.Posts.FindAsync(PostId);
            if (post is null)
                throw new KeyNotFoundException("The Post Not Found");

            return await _genericRepository.DeleteAsync(PostId);
        }

        public async Task<PostResponse?> GetAsync(Guid PostId)
        {
            return await _context.Posts.Select(p => new PostResponse
            {
                Id = p.Id,
                UserId = p.UserId,
                BookId = p.BookId,
                CreatedAt = p.CreatedAt,
            }).FirstOrDefaultAsync(p => p.Id == PostId);
        }

        public async Task<int> UpdateAsync(Guid PostId, Post NewEntity)
        {
            var post = await _context.Posts.FindAsync(PostId);
            var book = await _context.Books.FindAsync(NewEntity.BookId);
            if (post is null || book is null)
                throw new KeyNotFoundException($"Post Or User Or User not found.");

            post.BookId = NewEntity.BookId;
            post.UpdatedAt = NewEntity.UpdatedAt;

            return await _genericRepository.UpdateAsync(PostId, post);
        }
    }
}
