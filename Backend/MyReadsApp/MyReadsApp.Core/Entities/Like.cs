using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Entities
{
    public class Like
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public DateTime CreatedAt { get; set; }
        

        public User User { get; set; }
        public Post Post { get; set; }
    }
}
