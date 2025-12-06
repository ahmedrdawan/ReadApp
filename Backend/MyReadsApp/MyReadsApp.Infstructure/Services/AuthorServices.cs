using MyReadsApp.Core.DTOs.Author.Response;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class AuthorServices : IAuthorServices
    {
        private readonly IGenericRepository<Author> _genericRepository;
        private readonly AppDbContext _context;
        public AuthorServices(IGenericRepository<Author> genericRepository, AppDbContext context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<int> CreateAsync(Author entity)
        {
            var author = _context.Authors.FirstOrDefault(a=>a.AuthorName == entity.AuthorName);
            if (author != null)
                throw new FoundException("The Author Alreday Exist");

            return await _genericRepository.CreateAsync(entity);
        }
        public async Task<int> DeleteAsync(Guid UserId) => await _genericRepository.DeleteAsync(UserId);
        public async Task<AuthorResponse?> GetAsync(Guid AuthorId) 
        {
            return await _context.Authors.Select(a => new AuthorResponse
            {
                Id = a.Id,
                AuthorImage = a.AuthorImage,
                Bio = a.Bio,
                AuthorName = a.AuthorName
            }).FirstOrDefaultAsync(a => a.Id == AuthorId);
        }

        public async Task<int> UpdateAsync(Guid UserId, Author NewEntity) => await _genericRepository.UpdateAsync(UserId, NewEntity);
    }
}
