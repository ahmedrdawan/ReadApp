namespace MyReadsApp.Core.Entities
{
    public class Author
    {
        public Guid Id { get; set; }
        public string? AuthorImage { get; set; }
        public string? AuthorName { get; set; }
        public string? Bio { get; set; }

        public ICollection<Book> Books { get; set; }
    }
}
