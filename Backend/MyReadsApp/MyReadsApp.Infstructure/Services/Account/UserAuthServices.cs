using Microsoft.AspNetCore.Http;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Services.Interfaces.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services.Account
{
    public class UserAuthServices : IUserAuthServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetCurrentUser()
        {
            var claim = _httpContextAccessor.HttpContext?
            .User?.FindFirst(ClaimTypes.NameIdentifier);

            if (claim == null)
                throw new UnauthorizedAccessException("User is not authenticated");

            return Guid.Parse(claim?.Value!);
        }
    }
}
