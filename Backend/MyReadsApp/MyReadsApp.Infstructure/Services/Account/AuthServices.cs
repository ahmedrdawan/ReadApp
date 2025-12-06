using MyReadsApp.API.DTOs;
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth.Request;
using MyReadsApp.Core.DTOs.Auth.Response;
using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Exceptions;
using MyReadsApp.Core.Services.Interfaces.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class AuthServices : IAuthServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtTokenServices _IJwtTokenServices;
        private readonly IEmailservices _emailservices;
        private readonly IConfiguration _configration;

        public AuthServices(UserManager<User> userManager, SignInManager<User> signInManager, IJwtTokenServices iJwTokenServices, IEmailservices emailservices, IConfiguration configration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _IJwtTokenServices = iJwTokenServices;
            _emailservices = emailservices;
            _configration = configration;
        }

        public async Task<Response<AuthResponse>> RegisterAsync(RegisterRequest request)
        {
            User? Existuser = await _userManager.FindByEmailAsync(request.Email);
            if (Existuser != null)
                return Response<AuthResponse>.Failure(error: "User with this email already exists.", statusCode: 409);

            Existuser = await _userManager.FindByNameAsync(request.UserName);
            if (Existuser != null)
                return Response<AuthResponse>.Failure(error: "User with this UserName already exists.", statusCode: 409);

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                CreatedAt = DateTime.Now,
                Role = "User"
            };

            var resultUser = await _userManager.CreateAsync(user, request.Password);
            if (!resultUser.Succeeded)
                return Response<AuthResponse>.Failure((resultUser.Errors.Select(e => e.Description).ToList()), 500);

            //var ConfirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //var encodeEmailToken = Encoding.UTF8.GetBytes(ConfirmEmailToken);
            //var ValidEmailToken = WebEncoders.Base64UrlEncode(encodeEmailToken);

            //string url = $"{_configration["appURL"]}api/Auth/confirm-email?userId={user.Id}&code={ValidEmailToken}";
            //await _emailservices.SendEmailAsync(user.Email, "ConfirmEmail",$"<h1>Email Body {url}</h1>" );
            

            var resultRole = await _userManager.AddToRoleAsync(user, user.Role);
            if (!resultRole.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return Response<AuthResponse>.Failure("Failed to assign default role.", 500);
            }

            TokenResult Token = await _IJwtTokenServices.GenerateJwtTokenAsync(user);
            var response = BuildAuthResponse(user, Token);
            return Response<AuthResponse>.Sucess(response, 201);
        }

        public async Task<Response<AuthResponse>> LoginAsync(LoginRequest request)
        {
            User? Existuser = await _userManager.FindByEmailAsync(request.Email);
            if (Existuser == null)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);

            bool IsValid = await _userManager.CheckPasswordAsync(Existuser, request.Password);
            if (!IsValid)
                return Response<AuthResponse>.Failure("Invalid email or password.", 401);


            TokenResult Token = await _IJwtTokenServices.GenerateJwtTokenAsync(Existuser);
            var response = BuildAuthResponse(Existuser, Token);

            return Response<AuthResponse>.Sucess(response);

        }
        private static AuthResponse BuildAuthResponse(User user, TokenResult tokenResult)
        {
            return new AuthResponse(
                new UserDTO(
                    user.Id,
                    user.Email,
                    user.UserName,
                    user.Role),
                new TokenResult(
                    tokenResult.Token,
                    tokenResult.ExpiresAt)
                );
        }

    }
}
