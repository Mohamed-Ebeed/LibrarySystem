using AutoMapper;
using LibrarySystem.Results;

namespace LibrarySystem.Helper
{
    public static class PaginatedListExtensions
    {
        
        public static PaginatedList<TDto> MapTo<TEntity, TDto>(this PaginatedList<TEntity> source, IMapper mapper)
            => new PaginatedList<TDto>
            {
                PageNumber = source.PageNumber,
                PageSize = source.PageSize,
                TotalRecords = source.TotalRecords,
                Data = mapper.Map<IEnumerable<TDto>>(source.Data)
            };
    }
}