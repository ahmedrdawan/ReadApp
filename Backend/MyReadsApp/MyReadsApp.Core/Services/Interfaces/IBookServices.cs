using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.Book.Response;
using MyReadsApp.Core.DTOs.Book.Request;
using MyReadsApp.Core.Entities;
using MyReadsApp.Core.Generic.Interfaces;

namespace MyReadsApp.Core.Services.Interfaces
{
    /// <summary>
    /// Provides operations for managing books, including CRUD, search, and ratings.
    /// </summary>
    public interface IBookServices
    {
        /// <summary>
        /// Creates a new book.
        /// </summary>
        /// <param name="book">Book entity to create.</param>
        /// <returns>
        /// A task returning a Response containing the created BookAuthorResponse.
        /// </returns>
        Task<Response<BookAuthorResponse>> CreateAsync(CreateBookRequest request);

        /// <summary>
        /// Deletes a book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book to delete.</param>
        /// <returns>
        /// A task returning a Response containing the deleted BookAuthorResponse or error.
        /// </returns>
        Task<Response<BookAuthorResponse>> DeleteAsync(Guid BookId);

        /// <summary>
        /// Updates an existing book.
        /// </summary>
        /// <param name="id">The unique identifier of the book to update.</param>
        /// <param name="newBook">Updated book data.</param>
        /// <returns>
        /// A task returning a Response containing the updated BookAuthorResponse.
        /// </returns>
        Task<Response<BookAuthorResponse>> UpdateAsync(Guid id, UpdateBookRequest request);

        /// <summary>
        /// Retrieves a book by its identifier.
        /// </summary>
        /// <param name="BookId">The unique identifier of the book.</param>
        /// <returns>
        /// A task returning a Response containing the BookAuthorResponse.
        /// </returns>
        Task<Response<BookAuthorResponse>> GetAsync(Guid BookId);

        /// <summary>
        /// Retrieves a paginated list of books, optionally filtered by category.
        /// </summary>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="categoryId">Optional category identifier to filter results.</param>
        /// <returns>
        /// A task returning a Response containing a paged result of BookAuthorResponse.
        /// </returns>
        Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> GetListAsync(int pageNumber = 1, int pageSize = 10, Guid? categoryId = null);

        /// <summary>
        /// Searches for books by query string with pagination.
        /// </summary>
        /// <param name="query">Search query string.</param>
        /// <param name="pageNumber">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="categoryId">Optional category filter.</param>
        /// <returns>
        /// A task returning a Response containing a paged result of BookAuthorResponse matching the query.
        /// </returns>
        Task<Response<MyReadsApp.Core.Common.PagedResult<BookAuthorResponse>>> SearchAsync(string query, int pageNumber = 1, int pageSize = 10, Guid? categoryId = null);

        /// <summary>
        /// Adds or updates a rating for a book by a user.
        /// </summary>
        /// <param name="bookId">The book identifier to rate.</param>
        /// <param name="userId">The user identifier providing the rating.</param>
        /// <param name="value">Rating value.</param>
        /// <returns>
        /// A task returning a Response with operation result object.
        /// </returns>
        Task<Response<object>> RateBookAsync(Guid bookId, Guid userId, int value);

        /// <summary>
        /// Retrieves rating summary for a book, optionally including the specified user's rating.
        /// </summary>
        /// <param name="bookId">The book identifier.</param>
        /// <param name="userId">Optional user identifier to include user-specific rating.</param>
        /// <returns>
        /// A task returning a Response with rating summary object.
        /// </returns>
        Task<Response<object>> GetRatingSummaryAsync(Guid bookId, Guid? userId = null);
    }
}
