using LibrarySystem.Dtos.Author;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public interface IAuthorService
    {
        Task<GenericResult<PaginatedList<AuthorResponse>>> GetAllAsync(int pageNumber, int pageSize, string? search, DateTime? bornFrom, DateTime? bornTo);
        Task<GenericResult<AuthorResponse>> GetByIdAsync(int id);
        Task<GenericResult<AuthorResponse>> CreateAsync(CreateAuthorRequest request);
        Task<GenericResult<AuthorResponse>> UpdateAsync(UpdateAuthorRequest request);
        Task<Result> DeleteAsync(int id);
    }
}