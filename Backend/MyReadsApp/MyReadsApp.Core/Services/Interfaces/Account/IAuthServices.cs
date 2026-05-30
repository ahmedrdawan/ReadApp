
using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Auth;
using MyReadsApp.Core.DTOs.Auth.Response;


namespace MyReadsApp.Core.Services.Interfaces.Account
{
    /// <summary>
    /// Defines authentication-related service operations.
    /// </summary>
    public interface IAuthServices
    {
        /// <summary>
        /// Registers a new user account asynchronously.
        /// </summary>
        /// <param name="request">Registration details including username, email, and password.</param>
        /// <returns>
        /// A task representing the asynchronous operation. The task result contains a Response indicating success or failure.
        /// </returns>
        Task<Response> RegisterAsync(RegisterDto request);

        /// <summary>
        /// Authenticates a user asynchronously and returns authentication tokens.
        /// </summary>
        /// <param name="request">Login credentials containing email and password.</param>
        /// <returns>
        /// A task that returns a Response&lt;AuthResponse&gt; containing JWT and refresh token on success.
        /// </returns>
        Task<Response<AuthResponse>> LoginAsync(LoginDto request);

        /// <summary>
        /// Confirms a user's email address using a verification token.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <param name="token">Email confirmation token.</param>
        /// <returns>
        /// A task containing a Response indicating success or failure of confirmation.
        /// </returns>
        Task<Response> ConfirmEmailAsync(string email, string token);

        /// <summary>
        /// Generates a new authentication response using a valid refresh token.
        /// </summary>
        /// <returns>
        /// A task that returns a Response&lt;AuthResponse&gt; with refreshed tokens.
        /// </returns>
        Task<Response<AuthResponse>> RefreshTokenAsync();

        /// <summary>
        /// Performs Google external login using the provided email and optional name.
        /// </summary>
        /// <param name="email">The email address obtained from Google.</param>
        /// <param name="name">Optional display name from Google.</param>
        /// <returns>
        /// A task that returns a Response&lt;AuthResponse&gt; on successful external login.
        /// </returns>
        Task<Response<AuthResponse>> GoogleLoginAsync(string email, string? name);

        /// <summary>
        /// Initiates forgot-password flow by sending a reset token to the provided email.
        /// </summary>
        /// <param name="email">User email address.</param>
        /// <returns>
        /// A task containing a Response indicating whether the reset email was sent.
        /// </returns>
        Task<Response> ForgotPasswordAsync(string email);

        /// <summary>
        /// Resets the user password using the provided reset token and new password.
        /// </summary>
        /// <param name="request">Reset password details including email, token, and new password.</param>
        /// <returns>
        /// A task containing a Response indicating success or failure of the password reset.
        /// </returns>
        Task<Response> ResetPasswordAsync(ResetPasswordDto request);
    }
}
