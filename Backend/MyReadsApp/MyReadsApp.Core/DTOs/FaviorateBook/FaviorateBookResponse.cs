using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.FaviorateBook
{
    public class FaviorateBookResponse
    {
        public Guid UserId { get; set; }
        public Guid BookId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
