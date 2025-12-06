using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Entities
{
    public class FaviorateBook
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }

        DateTime? CreatedAt { get; set; }


        public User User { get; set; }
        public Book Book { get; set; }
    }
}
