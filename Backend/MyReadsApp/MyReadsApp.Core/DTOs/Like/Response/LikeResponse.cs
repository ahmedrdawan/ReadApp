using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Like.Response
{
    public class LikeResponse
    {
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
