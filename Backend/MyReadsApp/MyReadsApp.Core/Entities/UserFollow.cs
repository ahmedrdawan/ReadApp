using MyReadsApp.Core.Entities.Identity;

namespace MyReadsApp.Core.Entities
{
    public class UserFollow
    {
        public Guid Id { get; set; }
        public Guid FollowerId { get; set; } // SEND
        public Guid FollowingId { get; set; } // (received)
        public DateTime CreatedAt { get; set; }

        public User UserFollowers { get; set; }
        public User UserFollowing { get; set; }

    }
}
