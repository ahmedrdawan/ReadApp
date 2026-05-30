using MyReadsApp.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    /// <summary>
    /// Provides helper methods to obtain authenticated user information from context.
    /// </summary>
    public interface IUserAuthServices
    {
        /// <summary>
        /// Gets the current authenticated user's identifier.
        /// </summary>
        /// <returns>The Guid representing the current user.</returns>
        Guid GetCurrentUser();
    }
}
