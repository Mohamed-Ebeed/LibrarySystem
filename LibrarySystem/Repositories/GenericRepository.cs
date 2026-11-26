using LibrarySystem.Domain;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Results;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibrarySystem.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly LibraryDbContext _dbContext;

        public GenericRepository(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private IQueryable<T> Query(Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            foreach (var include in includes)
                query = query.Include(include);
            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
            => await Query(includes).AsNoTracking().ToListAsync();

        public async Task<PaginatedList<T>> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes)
        {
            var query = Query(includes).AsNoTracking();

            if (filter is not null)
                query = query.Where(filter);

            var result = new PaginatedList<T>
            {
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize
            };

            result.TotalRecords = await query.CountAsync();
            result.Data = await query
                .OrderBy(e => e.Id)
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize)
                .ToListAsync();

            return result;
        }

        public async Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes)
            => await Query(includes).FirstOrDefaultAsync(e => e.Id == id);

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
            => await _dbContext.Set<T>().AnyAsync(predicate);

        public async Task<T> CreateAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(T entity)
        {
            var exists = await _dbContext.Set<T>().AnyAsync(e => e.Id == entity.Id);
            if (!exists)
                return false;

            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbContext.Set<T>().FindAsync(id);
            if (entity is null || entity.IsDeleted)
                return false;

            _dbContext.Set<T>().Remove(entity);   // الـ SoftDeleteInterceptor بيحوّله لـ IsDeleted = true
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}