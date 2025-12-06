using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MyReadsApp.Infstructure.Services
{
    public class BookServices : IBookServices
    {
        private readonly IGenericRepository<Book> _genericRepository;
        private readonly AppDbContext _context;
        public BookServices(IGenericRepository<Book> genericRepository, AppDbContext context)
        {
            _genericRepository = genericRepository;
            _context = context;
        }

        public async Task<int> CreateAsync(Book entity)
        {
            var author = await _context.Authors.FindAsync(entity.AuthorId);
            if (author == null)
                throw new NoFoundException("The Author Not Found");

            var exists = await _context.Books
                .AnyAsync(x => x.Title == entity.Title && x.AuthorId == entity.AuthorId);

            if (exists)
                throw new ConfilectException("The Book Already Exist");

            return await _genericRepository.CreateAsync(entity);
        }
        public async Task<int> DeleteAsync(Guid BookId) => await _genericRepository.DeleteAsync(BookId);
        //public async Task<Book?> GetAsync(Guid BookId)
        //{
        //    return await _genericRepository.GetAsync(BookId);
        //}

        public async Task<BookAuthorResponse?> GetAsync(Guid BookId)
        {
            return await _context.Books.Select(b => new BookAuthorResponse
            {
                Id = b.Id,
                BookImage = b.BookImage,
                Description = b.Description,
                AuthorId = b.AuthorId,
                Title = b.Title
            }).FirstOrDefaultAsync(b => b.Id == BookId);
        }

        public async Task<int> UpdateAsync(Guid id, Book newEntity)
        {
            var entity = await _context.Books.FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"Book with Id '{id}' not found.");

            var authorExists = await _context.Authors.AnyAsync(a => a.Id == newEntity.AuthorId);
            if (!authorExists)
                throw new KeyNotFoundException($"Author with Id '{newEntity.AuthorId}' not found.");

            entity.BookImage = newEntity.BookImage;
            entity.Description = newEntity.Description;
            entity.Title = newEntity.Title;
            entity.AuthorId = newEntity.AuthorId;

            return await _genericRepository.UpdateAsync(id, entity);
        }
    }
}
