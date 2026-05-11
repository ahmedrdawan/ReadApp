namespace MyReadsApp.Core.Entities
{
    public class BookCategory
    {
        public Guid BookId { get; set; }
        public Guid CategoryId { get; set; }

        public Book Book { get; set; }
        public Category Category { get; set; }
    }
}
