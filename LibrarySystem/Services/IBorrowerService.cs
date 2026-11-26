using LibrarySystem.Dtos.Borrower;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public interface IBorrowerService
    {
        Task<GenericResult<PaginatedList<BorrowerResponse>>> GetAllAsync(int pageNumber, int pageSize, string? search, DateTime? memberSince);
        Task<GenericResult<BorrowerResponse>> GetByIdAsync(int id);
        Task<GenericResult<BorrowerResponse>> CreateAsync(CreateBorrowerRequest request);
        Task<GenericResult<BorrowerResponse>> UpdateAsync(UpdateBorrowerRequest request);
        Task<Result> DeleteAsync(int id);
    }
}