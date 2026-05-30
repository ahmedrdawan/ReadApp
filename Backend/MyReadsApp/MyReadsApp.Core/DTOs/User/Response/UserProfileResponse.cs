using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.DTOs.User.Response
{
    /// <summary>
    /// Response DTO for user profile information containing user details.
    /// </summary>
    public class UserProfileResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the country of the user.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        public Gender? Gender { get; set; }

        /// <summary>
        /// Gets or sets the birth date of the user.
        /// </summary>
        public DateTime? BirthDate { get; set; }

        /// <summary>
        /// Gets or sets the URL or path to the user's profile image.
        /// </summary>
        public string? UserImage { get; set; }
    }
}
