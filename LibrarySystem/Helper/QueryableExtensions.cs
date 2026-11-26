using LibrarySystem.Results;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Helper
{
    public static class QueryableExtensions
    {
        
        public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            var result = new PaginatedList<T>
            {
                PageNumber = pageNumber < 1 ? 1 : pageNumber,
                PageSize = pageSize
            };

            result.TotalRecords = await query.CountAsync();
            result.Data = await query
                .Skip((result.PageNumber - 1) * result.PageSize)
                .Take(result.PageSize)
                .ToListAsync();

            return result;
        }
    }
}