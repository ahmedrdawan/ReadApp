using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;

namespace MyReadsApp.Infstructure.Services
{
    public class FaviroteBookServices : IFaviroteBookServices
    {
        private readonly IGenericRepository<FaviorateBook> _repository;
        private readonly AppDbContext _context;
        private readonly IUserAuthServices _userAuthServices;

        public FaviroteBookServices(IGenericRepository<FaviorateBook> repository, AppDbContext context, IUserAuthServices userAuthServices)
        {
            _repository = repository;
            _context = context;
            _userAuthServices = userAuthServices;
        }

        public async Task<int> CreateAsync(FaviorateBook entity)
        {
            var user = await _context.Users.FindAsync(entity.UserId);
            if (user == null)
                throw new NotFoundException("The User Not Found");

            if (user.Id != _userAuthServices.GetCurrentUser())
                throw new NotAuthorizeException("The User Not Authorize");

            var book = await _context.Books.FindAsync(entity.BookId);
            if (book == null)
                throw new NotFoundException("The Book Not Found");

            var ExistEntity = await _context.FaviorateBooks
                .FirstOrDefaultAsync(f => f.UserId == entity.UserId && f.BookId == entity.BookId);


            if (ExistEntity != null)
                throw new ConfilectException("The Faviorate Book Already Exist");

            return await _repository.CreateAsync(entity);
        }

        public async Task<int> DeleteAsync(Guid BookId)
        {
            var userId = _userAuthServices.GetCurrentUser();

            var favbook = await _context.FaviorateBooks
                .FirstOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == BookId);

            if (favbook == null)
                throw new NotFoundException("The Faviorate Book Not Found");

            _context.Remove(favbook);
            return await _context.SaveChangesAsync();
        }

        public async Task<FaviorateBookResponse?> GetFavBookAsync(Guid BookId)
        {
            var userId = _userAuthServices.GetCurrentUser();

            return await _context.FaviorateBooks
                .Where(fb => fb.UserId == userId && fb.BookId == BookId)
                .Select(fb => new FaviorateBookResponse
                {
                    BookId = fb.BookId,
                    UserId = fb.UserId,
                    CreatedAt = fb.CreatedAt,
                }).FirstOrDefaultAsync();
        }
    }
}
