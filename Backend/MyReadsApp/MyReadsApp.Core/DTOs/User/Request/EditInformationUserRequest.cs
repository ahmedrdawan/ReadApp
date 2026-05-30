using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.DTOs.User.Request
{
    /// <summary>
    /// Request DTO for editing user profile information.
    /// </summary>
    public class EditInformationUserRequest
    {
        /// <summary>
        /// Gets or sets the new username.
        /// </summary>
        public string? UserName { get; set; }

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
    }
}
