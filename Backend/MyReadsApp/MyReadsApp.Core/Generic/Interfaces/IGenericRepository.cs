using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.Generic.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //Task<T?> GetAsync(Guid id);
        Task<int> CreateAsync(T entity);
        Task<int> UpdateAsync(Guid Id, T NewEntity);
        Task<int> DeleteAsync(Guid Id);
    }
}
