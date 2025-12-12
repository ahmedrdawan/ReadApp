using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly AppDbContext _content;

        public GenericRepository(AppDbContext content)
        {
            _content = content;
        }

        public async Task<int> CreateAsync(T entity)
        {
            await _content.Set<T>().AddAsync(entity);
            return await _content.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(T entity)
        {
            _content.Set<T>().Remove(entity);

            return await _content.SaveChangesAsync();
        }
        //public async Task<T?> GetAsync(Guid id)
        //{
        //    return await _content.Set<T>().FindAsync(id);
        //}

        public async Task<int> UpdateAsync(T NewEntity)
        {
            _content.Set<T>().Update(NewEntity);
            return await _content.SaveChangesAsync();
        }
    }
}
