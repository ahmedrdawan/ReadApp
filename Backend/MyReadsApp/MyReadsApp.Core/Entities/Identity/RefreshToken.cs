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
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
        public bool IsActive => DateTime.UtcNow < ExpireAt;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid UserId { get; set; }
        public User User { get; set; } 
    }
}
