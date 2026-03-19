using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.DTOs.FileStorage
{
    public class FileStorageRequest
    {
        public IFormFile File { get; set; }
    }
}
