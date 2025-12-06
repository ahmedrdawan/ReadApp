namespace MyReadsApp.API.DTOs.Post
{
    public class CreatedPostRequest: BasePostRequest
    {
        public Guid UserId { get; set; }
    }

}
