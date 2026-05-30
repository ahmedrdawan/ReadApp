namespace MyReadsApp.Core.DTOs.Auth.Response
{
    /// <summary>
    /// DTO for user information containing user profile details.
    /// </summary>
    public record UserDTO
    (
        /// <summary>
        /// Gets the unique identifier of the user.
        /// </summary>
        Guid Id,

        /// <summary>
        /// Gets the email address of the user.
        /// </summary>
        string Email,

        /// <summary>
        /// Gets the username of the user.
        /// </summary>
        string UserName,

        /// <summary>
        /// Gets the role of the user (e.g., User, Admin).
        /// </summary>
        string Role
    );
}
