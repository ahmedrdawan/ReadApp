using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FaviorateBook;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;
using MyReadsApp.Infstructure.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<Response<FaviorateBookResponse>> CreateAsync(FaviorateBook entity)
        {
            var user = await _context.Users.FindAsync(entity.UserId);
            if (user == null)
                return Response<FaviorateBookResponse>.Failure("The User Not Found", 404);

            if (user.Id != _userAuthServices.GetCurrentUser())
                return Response<FaviorateBookResponse>.Failure("The User Not Authorized", 403);

            var book = await _context.Books.FindAsync(entity.BookId);
            if (book == null)
                return Response<FaviorateBookResponse>.Failure("The Book Not Found", 404);

            var existing = await _context.FaviorateBooks
                .AnyAsync(f => f.UserId == entity.UserId && f.BookId == entity.BookId);

            if (existing)
                return Response<FaviorateBookResponse>.Failure("The Favorite Book Already Exists", 409);

            await _repository.CreateAsync(entity);
            FaviorateBookResponse response = BuildResponse(entity);

            return Response<FaviorateBookResponse>.Success(response);
        }

        private static FaviorateBookResponse BuildResponse(FaviorateBook entity)
        {
            return new FaviorateBookResponse
            {
                UserId = entity.UserId,
                BookId = entity.BookId,
                CreatedAt = entity.CreatedAt,
            };
        }

        public async Task<Response<FaviorateBookResponse>> DeleteAsync(Guid bookId)
        {
            var userId = _userAuthServices.GetCurrentUser();

            var favBook = await _context.FaviorateBooks
                .FirstOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == bookId);

            if (favBook == null)
                return Response<FaviorateBookResponse>.Failure("The Favorite Book Not Found", 404);

            _context.FaviorateBooks.Remove(favBook);

            var response = BuildResponse(favBook);

            return Response<FaviorateBookResponse>.Success(response);
        }

        public async Task<Response<FaviorateBookResponse>> GetFavBookAsync(Guid bookId)
        {
            var userId = _userAuthServices.GetCurrentUser();

            var favBook = await _context.FaviorateBooks
                .SingleOrDefaultAsync(fb => fb.UserId == userId && fb.BookId == bookId);

            if (favBook == null)
                return Response<FaviorateBookResponse>.Failure("Favorite Book Not Found", 404);
            
               
            var response = BuildResponse(favBook);

            return Response<FaviorateBookResponse>.Success(response);
        }
    }
}
