using System.ComponentModel.DataAnnotations;

namespace MyReadsApp.Core.DTOs.Auth
{
    public record ConfirmEmailDto(string UserId, string code);
}
