using MyReadsApp.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IUserAuthServices
    {
        Guid GetCurrentUser();
    }
}
