using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.UserFollow.Response
{
    public class UserFollowResponse
    {
        public string SendUserFullName { get; set; } // SEND
        public string ReceiveUserFullName { get; set; } // (received)
        public DateTime CreatedAt { get; set; }
    }
}
