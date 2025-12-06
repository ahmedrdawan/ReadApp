namespace MyReadsApp.Core.Entities
{
    public class Book
    {
        public Guid Id { get; set; }
        public string? BookImage { get; set; }
        public string? Description { get; set; }
        public string? Title { get; set; }
        public int rated { get; set; }
        public string? Content { get; set; }
        public Guid AuthorId { get; set; }

        public Author Author { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<FaviorateBook> FaviorateBooks { get; set; }
        public ICollection<UserBook> UserBooks { get; set; }
    }
}
