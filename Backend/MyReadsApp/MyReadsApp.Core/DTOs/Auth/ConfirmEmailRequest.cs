namespace MyReadsApp.API.DTOs
{
    public record ConfirmEmailRequest(string UserId, string code);
}
