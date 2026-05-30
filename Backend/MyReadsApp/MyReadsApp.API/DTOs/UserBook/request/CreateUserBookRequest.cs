using MyReadsApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.UserBook.request
{
    /// <summary>
    /// Request DTO for creating a user book containing book status.
    /// </summary>
    public class CreateUserBookRequest
    {
        /// <summary>
        /// Gets or sets the status of the user's book.
        /// </summary>
        [Required]
        public UserBookStatus Statuts { get; set; }
    }

    /// <summary>
    /// Request DTO for updating a user book.
    /// </summary>
    public class UpdateUserBookRequest
    {
        /// <summary>
        /// Gets or sets the updated status of the user's book.
        /// </summary>
        [Required]
        public UserBookStatus Statuts { get; set; }
    }
}
