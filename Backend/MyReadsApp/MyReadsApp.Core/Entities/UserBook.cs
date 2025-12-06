using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.Entities
{
    public class UserBook
    {
        public Guid Id { get; set; }
        public Guid BookId{ get; set; }
        public Guid UserId{ get; set; }
        public DateTime CreatedAt { get; set; }
        public UserBookStatus Statuts { get; set; }


        public User User { get; set; }
        public Book Book { get; set; }

    }
}
