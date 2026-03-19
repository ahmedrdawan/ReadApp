using MyReadsApp.Core.Enums;

namespace MyReadsApp.Core.DTOs.User.Response
{
    public class UserProfileResponse
    {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Country { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? UserImage { get; set; }
    }
}
