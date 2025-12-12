using MyReadsApp.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.UserBook.Response
{
    public class UserBookResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserBookStatus Statuts { get; set; }
    }
}
