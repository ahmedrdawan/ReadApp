using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Auth
{
    public record VerfyResetTokenDto
    (string Email, string Token);
}
