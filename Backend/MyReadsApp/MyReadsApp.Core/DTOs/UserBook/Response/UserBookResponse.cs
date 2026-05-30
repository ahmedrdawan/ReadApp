using MyReadsApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.UserBook.Response
{
    /// <summary>
    /// Response DTO for user book information containing book association details.
    /// </summary>
    public class UserBookResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user book record.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the creation date of the user book record.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the status of the book for the user (e.g., Reading, Completed).
        /// </summary>
        public UserBookStatus Statuts { get; set; }
    }
}
