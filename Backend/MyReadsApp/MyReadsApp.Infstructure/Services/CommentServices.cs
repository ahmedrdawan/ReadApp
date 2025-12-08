using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.Comment.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class CommentServices : ICommentServises
    {
        private readonly IGenericRepository<Comment> _repository;
        private readonly IUserAuthServices _userAuthServices;
        private readonly AppDbContext context;

        public CommentServices(IGenericRepository<Comment> repository, AppDbContext context, IUserAuthServices userAuthServices)
        {
            _repository = repository;
            this.context = context;
            _userAuthServices = userAuthServices;
        }

        public async Task<int> CreateAsync(Comment entity)
        {
            var user = await context.Users.FindAsync(entity.UserId);
            if (user == null) 
                throw new NotFoundException("The User Not Found");

            var post = await context.Posts.FindAsync(entity.PostId);
            if (post == null)
                throw new NotFoundException("The Post Not Found");

            return await _repository.CreateAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid CommentId)
        {
            var comment = await context.Comments.FindAsync(CommentId);

            if (comment == null)
                throw new NotFoundException("The Comment Not Found");

            if (comment.UserId != _userAuthServices.GetCurrentUser())
                throw new NotAuthorizeException("The User Not Authorize");

            return await _repository.DeleteAsync(CommentId);
        }

        public async Task<int> UpdateAsync(Guid CommentId, Comment NewEntity)
        {
            var comment = await context.Comments.FindAsync(CommentId);
            if (comment == null)
                throw new NotFoundException("The Comment Not Found");

            if (comment.UserId != _userAuthServices.GetCurrentUser())
                throw new NotAuthorizeException("The User Not Authorize");

            comment.content = NewEntity.content;
            comment.UpdatedAt = DateTime.UtcNow;

            return await _repository.UpdateAsync(CommentId, comment);
        }

        public async Task<CommentResponse?> GetAsync(Guid CommentId)
        {
            return await context.Comments
                .Where(c=> c.Id == CommentId)
                .Select(c => new CommentResponse
                {
                    content = c.content,
                    UserId = c.UserId,
                    PostId = c.PostId,
                })
                .FirstOrDefaultAsync();
        }
    }
}
