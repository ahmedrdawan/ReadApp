using MyReadsApp.Core.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Entities.Identity
{
    public class User : IdentityUser<Guid>
    {
        public Gender? Gender { get; set; } = null;
        public string? Country { get; set; }
        public string? UserImage { get; set; }

        public string Role { get; set; } = "User";
        public DateTime? BirthDate { get;set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<FaviorateBook> FaviorateBooks { get; set; }
        public ICollection<FriendShip> SentFriendShips { get; set; }
        public ICollection<FriendShip> ReceivedFriendShips { get; set; }
        public ICollection<UserBook> UserBooks { get; set; }
        public ICollection<UserFollow> UserFollowers { get; set; }
        public ICollection<UserFollow> UserFollowings { get; set; }
    }
}
