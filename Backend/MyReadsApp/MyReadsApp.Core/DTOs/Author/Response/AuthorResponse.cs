using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.Author.Response
{
    public class AuthorResponse
    {
        public Guid Id { get; set; }
        public string? AuthorImage { get; set; }
        public string? AuthorName { get; set; }
        public string? Bio { get; set; }
    }
}
