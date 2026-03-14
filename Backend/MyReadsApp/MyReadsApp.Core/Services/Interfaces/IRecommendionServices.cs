using MyReadsApp.Core.DTOs.FriendShip.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IRecommendionServices
    {
        Task<Core.Common.Response<List<FriendResponse>>> FriendsSuggestionAsync();
    }
}
