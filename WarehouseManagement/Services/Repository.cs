using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WarehouseManagement.Data;

namespace WarehouseManagement.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly WarehouseDbContext _context;

        public Repository(WarehouseDbContext context)
        {
            _context = context;
        }

        // Новый метод для IQueryable
        public IQueryable<T> GetAll()
        {
            return _context.Set<T>().AsQueryable();
        }

        // Существующие методы
        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public async Task<T> GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id);

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }
    }
}