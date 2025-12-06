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
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), $"The {typeof(T).Name} is null");

            await _content.Set<T>().AddAsync(entity);
            return await _content.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var entity = await _content.Set<T>().FindAsync(id);

            if (entity == null)
                throw new KeyNotFoundException($"Entity of type '{typeof(T).Name}' with Id '{id}' not found.");

            _content.Set<T>().Remove(entity);

            return await _content.SaveChangesAsync();
        }
        //public async Task<T?> GetAsync(Guid id)
        //{
        //    return await _content.Set<T>().FindAsync(id);
        //}

        public async Task<int> UpdateAsync(Guid UserId, T NewEntity)
        {
            var entity = await _content.Set<T>().FindAsync(UserId);

            if (entity == null)
                throw new KeyNotFoundException($"Entity of type '{typeof(T).Name}' with Id '{UserId}' not found.");

            entity = NewEntity;
            _content.Set<T>().Update(entity);
            return await _content.SaveChangesAsync();
        }
    }
}
