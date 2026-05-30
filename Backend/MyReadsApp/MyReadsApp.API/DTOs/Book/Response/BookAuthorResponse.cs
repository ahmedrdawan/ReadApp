namespace MyReadsApp.API.DTOs.Book.Response
{
    /// <summary>
    /// Response DTO for book details containing essential book information.
    /// </summary>
    public class BookAuthorResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the book.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the book.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description of the book.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the image URL or path for the book.
        /// </summary>
        public string BookImage { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the book's author.
        /// </summary>
        public Guid AuthorId { get; set; }
    }
}
