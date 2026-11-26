using LibrarySystem.Dtos.Loan;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public interface ILoanService
    {
        Task<GenericResult<PaginatedList<LoanResponse>>> GetAllAsync(int pageNumber, int pageSize, int? bookId, int? borrowerId, bool? isReturned);
        Task<GenericResult<LoanResponse>> GetByIdAsync(int id);
        Task<GenericResult<LoanResponse>> CreateAsync(CreateLoanRequest request);
        Task<GenericResult<LoanResponse>> UpdateAsync(UpdateLoanRequest request);
        Task<GenericResult<LoanResponse>> ReturnBookAsync(int id);
        Task<Result> DeleteAsync(int id);
    }
}