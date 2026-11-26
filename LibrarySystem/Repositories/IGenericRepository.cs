using LibrarySystem.Domain;
using LibrarySystem.Results;
using System.Linq.Expressions;

namespace LibrarySystem.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);

        
        Task<PaginatedList<T>> GetPagedAsync(int pageNumber, int pageSize,
            Expression<Func<T, bool>>? filter = null,
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T> CreateAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
    }
}