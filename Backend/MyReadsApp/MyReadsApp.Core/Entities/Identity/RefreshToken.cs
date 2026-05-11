using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Entities.Identity
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; } 

        public string Token { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpireAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsRevoked => RevokedAt != null;

        public bool IsExpired => DateTime.UtcNow >= ExpireAt;

        public bool IsActive => !IsRevoked && !IsExpired;

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
