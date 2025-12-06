using MyReadsApp.Core.Entities.Identity;
using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.Entities
{
    public class FriendShip
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } // SEND
        public Guid FriendId { get; set; } // (received)
        public DateTime CreatedAt { get; set; }
        public FriendShipStatus Status { get;set; }

        public User User { get; set; }
        public User FriendUser { get; set; }

    }
}
