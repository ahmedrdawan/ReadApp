using Microsoft.EntityFrameworkCore;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Like.Response;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class LikeServices : ILikeServices
    {
        private readonly IGenericRepository<Like> _likeRepository;
        private readonly AppDbContext _context;

        public LikeServices(IGenericRepository<Like> likeRepository, AppDbContext context)
        {
            _likeRepository = likeRepository;
            _context = context;
        }

        public async Task<Response<int>> CountLikeAsync(Guid postId)
        {
            var likes = await _context.Likes
                .CountAsync(l=>l.PostId == postId);

            if (likes == 0)
                return Response<int>.Failure("No Like For This Post", 404);

            return Response<int>.Success(likes);
        }

        public async Task<Response<LikeResponse>> CreateAsync(Like like)
        {
            var likeExisting = await _context.Likes
                .SingleOrDefaultAsync(l => l.UserId == like.UserId && l.PostId == like.PostId);

            if (likeExisting != null)
                return Response<LikeResponse>.Failure("The User Already Like This Post", 409);
            await _likeRepository.CreateAsync(like);
            return Response<LikeResponse>.Success(BuildResponse(like));
        }

        public async Task<Response<LikeResponse>> DeleteAsync(Guid postId, Guid userId)
        {
            var likeExisting = await _context.Likes
                .SingleOrDefaultAsync(l => l.UserId == userId && l.PostId == postId);

            if (likeExisting == null)
                return Response<LikeResponse>.Failure("The User Don't Like This Post", 404);
            await _likeRepository.DeleteAsync(likeExisting);

            return Response<LikeResponse>.Success(BuildResponse(likeExisting));
        }

        private static LikeResponse BuildResponse(Like like)
        {
            return new LikeResponse
            {
                PostId = like.PostId,
                UserId = like.UserId,
                CreatedAt = like.CreatedAt,
            };
        }
    }
}
