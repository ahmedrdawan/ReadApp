using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.DTOs.User.Request
{
    public class EditInformationUserRequest
    {
        public string? UserName { get; set; }
        public string? Country { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}
