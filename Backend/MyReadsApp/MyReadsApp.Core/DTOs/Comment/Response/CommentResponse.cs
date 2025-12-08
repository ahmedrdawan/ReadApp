using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Comment.Response
{
    public class CommentResponse
    {
        public Guid Id { get; set; }
        public string content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid PostId { get; set; }
    }
}
