using MyReadsApp.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.API.DTOs.UserBook.request
{
    public class CreateUserBookRequest
    {
        [Required]
        public UserBookStatus Statuts { get; set; }
    }
    public class UpdateUserBookRequest
    {
        [Required]
        public UserBookStatus Statuts { get; set; }
    }
}
