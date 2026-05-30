using System;

namespace MyReadsApp.Core.DTOs.UserBook
{
    /// <summary>
    /// Request DTO for updating a user book (shelf) entry in the core layer.
    /// </summary>
    public record UpdateUserBookRequest(
        /// <summary>
        /// The status of the user book (e.g., reading, completed).
        /// </summary>
        int Statuts
    );
}