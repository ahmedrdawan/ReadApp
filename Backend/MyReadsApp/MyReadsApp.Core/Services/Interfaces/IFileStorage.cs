using MyReadsApp.Core.Common;
using MyReadsApp.Core.DTOs.FileStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Services.Interfaces
{
    public interface IFileStorage
    {
        Task<Response<string>> UploadAsync(FileStorageRequest request);
        Task<Response> DeleteAsync(string fileName);
    }
}
