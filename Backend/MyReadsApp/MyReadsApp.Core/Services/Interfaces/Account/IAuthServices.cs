using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.DTOs.Auth.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces.Account
{
    public interface IAuthServices
    {
        Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request);
        Task<Response<AuthResponse>> LoginAsync(LoginRequest request);
    }
}
