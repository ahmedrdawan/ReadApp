using MyReadsApp.Core.Generic.Interfaces;
using MyReadsApp.Infstructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Infstructure.Services
{
    /// <summary>
    /// Generic repository implementation for common CRUD operations used by infrastructure services.
    /// Provides async create, read, update, and delete helpers abstracting the DbContext.
    /// </summary>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly AppDbContext _content;

        public GenericRepository(AppDbContext content)
        {
            _content = content;
        }

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to create.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> CreateAsync(T entity)
        {
            await _content.Set<T>().AddAsync(entity);
            return await _content.SaveChangesAsync();
        }

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> DeleteAsync(T entity)
        {
            _content.Set<T>().Remove(entity);

            return await _content.SaveChangesAsync();
        }
        //public async Task<T?> GetAsync(Guid id)
        //{
        //    return await _content.Set<T>().FindAsync(id);
        //}

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="NewEntity">The updated entity.</param>
        /// <returns>The number of state entries written to the database.</returns>
        public async Task<int> UpdateAsync(T NewEntity)
        {
            _content.Set<T>().Update(NewEntity);
            return await _content.SaveChangesAsync();
        }
    }
}
