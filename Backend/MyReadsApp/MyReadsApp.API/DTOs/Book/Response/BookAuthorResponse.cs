namespace MyReadsApp.API.DTOs.Book.Response
{
    public class BookAuthorResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string BookImage { get; set; }
        public Guid AuthorId { get; set; }
    }
}
