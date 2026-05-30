using System;

namespace MyReadsApp.Core.DTOs.UserBook
{
    /// <summary>
    /// Request DTO for creating a user book (shelf) entry in the core layer.
    /// </summary>
    public record CreateUserBookRequest(
        /// <summary>
        /// Identifier of the book to add to user's shelf.
        /// </summary>
        Guid BookId,

        /// <summary>
        /// Identifier of the user who owns this shelf entry.
        /// </summary>
        Guid UserId,

        /// <summary>
        /// The status of the user book (e.g., reading, completed).
        /// </summary>
        int Statuts
    );
}