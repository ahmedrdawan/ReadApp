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
    /// <summary>
    /// Provides access to the currently authenticated user information
    /// from the HTTP context.
    /// </summary>
    public class UserAuthServices : IUserAuthServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserAuthServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Retrieves the current authenticated user's unique identifier (GUID)
        /// from the JWT claims in the HTTP context.
        /// </summary>
        /// <returns>
        /// The user ID as a Guid.
        /// </returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Thrown when the user is not authenticated or claim is missing.
        /// </exception>
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
