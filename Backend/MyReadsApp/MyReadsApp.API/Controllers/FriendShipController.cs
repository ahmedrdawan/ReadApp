using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyReadsApp.API.DTOs.FriendShip;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Enums;
using MyReadsApp.Core.Services.Interfaces;
using MyReadsApp.Core.Services.Interfaces.Account;

namespace MyReadsApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendShipController : ControllerBase
    {
        private readonly IFriendshipServices _friendshipServices;
        private readonly IUserAuthServices _userAuthServices;

        public FriendShipController(IFriendshipServices friendshipServices, IUserAuthServices userAuthServices)
        {
            _friendshipServices = friendshipServices;
            _userAuthServices = userAuthServices;
        }

        [HttpPost("{friendId}")]
        public async Task<IActionResult> AddFriend(Guid friendId, CreateFriendShipRequest request)
        {
            var friendShip = new FriendShip
            {
                Id = Guid.NewGuid(),
                UserId = _userAuthServices.GetCurrentUser(),
                FriendId = friendId,
                CreatedAt = DateTime.UtcNow,
                Status = request.Status,
            };
            var result = await _friendshipServices.CreateAsync(friendShip);

            return StatusCode(result.StatusCode, result);
        }
        [HttpDelete("{friendId}")]
        public async Task<IActionResult> DeleteFriend(Guid friendId)
        {

            var result = await _friendshipServices.DeleteAsync(_userAuthServices.GetCurrentUser(), friendId);
            return StatusCode(result.StatusCode, result);

        }

        [HttpGet("accepted")]
        public async Task<IActionResult> GetFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.accepted);
            
            return Ok(result);
        }
        [HttpGet("bloked")]
        public async Task<IActionResult> GetBlokedFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.blocked);

            return Ok(result);
        }
        [HttpGet("pending")]
        public async Task<IActionResult> GetpendingFriends()
        {
            var result = await _friendshipServices
                .GetAllAsync((fh) => fh.UserId == _userAuthServices.GetCurrentUser() && fh.Status == FriendShipStatus.pending);

            return Ok(result);
        }
    }
}
