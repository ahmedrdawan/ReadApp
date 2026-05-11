namespace MyReadsApp.Core.Entities
{
    public class BookRating
    {
        public Guid Id { get; set; }
        public Guid BookId { get; set; }
        public Guid UserId { get; set; }
        public int Value { get; set; }
        public DateTime CreatedAt { get; set; }

        public Book Book { get; set; }
        public MyReadsApp.Core.Entities.Identity.User User { get; set; }
    }
}
