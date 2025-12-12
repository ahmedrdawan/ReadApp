using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.UserBook.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class UserBookServices : IUserBookServices
    {
        private readonly IGenericRepository<UserBook> _repository;
        private readonly AppDbContext _context;

        public UserBookServices(IGenericRepository<UserBook> repository, AppDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<Response<UserBookResponse>> CreateAsync(UserBook entity)
        {
            var userBookExisting = await _context.UserBooks
                .FirstOrDefaultAsync(us => us.UserId == entity.UserId && us.BookId == entity.BookId);

            if (userBookExisting != null)
                return Response<UserBookResponse>.Failure("The userBook Is already Exist", 409);

            await _repository.CreateAsync(entity);
            return Response<UserBookResponse>.Success(BuildResponse(entity));
        }

        public async Task<Response<UserBookResponse>> DeleteAsync(Guid Id)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            await _repository.DeleteAsync(userBookExisting);
            return Response<UserBookResponse>.Success(BuildResponse(userBookExisting));
        }

        public Response<IEnumerable<UserBookResponse>> GetAllAsync()
        {
            IQueryable<UserBookResponse> userBooks = _context.UserBooks
                .Select(us => BuildResponse(us));

            if (!userBooks.Any())
                return Response<IEnumerable<UserBookResponse>>.Failure("The userBook Is Not Found", 404);

            return Response<IEnumerable<UserBookResponse>>.Success(userBooks);
        }

        public async Task<Response<UserBookResponse>> GetByIdAsync(Guid Id)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);
            return Response<UserBookResponse>.Success(BuildResponse(userBookExisting));
        }

        public async Task<Response<UserBookResponse>> UpdateAsync(Guid Id, UserBook newEntity)
        {
            var userBookExisting = await _context.UserBooks.FindAsync(Id);
            if (userBookExisting == null)
                return Response<UserBookResponse>.Failure("The userBook Is Not Found", 404);

            userBookExisting.Statuts = newEntity.Statuts;
            await _repository.UpdateAsync(userBookExisting);

            return Response<UserBookResponse>.Success(BuildResponse(userBookExisting));
        }

        private static UserBookResponse BuildResponse(UserBook entity)
        {
            return new UserBookResponse
            {
                Id = entity.Id,
                CreatedAt = entity.CreatedAt,
                Statuts = entity.Statuts,
            };
        }
    }
}
