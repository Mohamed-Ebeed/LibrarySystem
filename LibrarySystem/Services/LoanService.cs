using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Loan;
using LibrarySystem.Helper;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public class LoanService(
        IGenericRepository<Loan> repository,
        IGenericRepository<Book> bookRepository,
        IGenericRepository<Borrower> borrowerRepository,
        IMapper mapper,
        ILogger<LoanService> logger) : ILoanService
    {
        public async Task<GenericResult<PaginatedList<LoanResponse>>> GetAllAsync(int pageNumber, int pageSize, int? bookId, int? borrowerId, bool? isReturned)
        {
            var loans = await repository.GetPagedAsync(pageNumber, pageSize,
                l => (bookId == null || l.BookId == bookId)
                  && (borrowerId == null || l.BorrowerId == borrowerId)
                  && (isReturned == null || (isReturned == true ? l.ReturnDate != null : l.ReturnDate == null)),
                l => l.Book, l => l.Borrower);

            logger.LogInformation("Got {Count} loans (page {Page})", loans.TotalRecords, loans.PageNumber);
            return GenericResult<PaginatedList<LoanResponse>>.Success(loans.MapTo<Loan, LoanResponse>(mapper));
        }

        public async Task<GenericResult<LoanResponse>> GetByIdAsync(int id)
        {
            var loan = await repository.GetByIdAsync(id, l => l.Book, l => l.Borrower);
            if (loan is null)
            {
                logger.LogWarning("Loan with Id = {Id} is not found", id);
                return GenericResult<LoanResponse>.NotFound($"Loan with Id = {id} is not found.");
            }

            return GenericResult<LoanResponse>.Success(mapper.Map<LoanResponse>(loan));
        }

        public async Task<GenericResult<LoanResponse>> CreateAsync(CreateLoanRequest request)
        {
            if (!await bookRepository.ExistsAsync(b => b.Id == request.BookId))
                return GenericResult<LoanResponse>.Failure($"Book with Id = {request.BookId} is not found.");

            if (!await borrowerRepository.ExistsAsync(b => b.Id == request.BorrowerId))
                return GenericResult<LoanResponse>.Failure($"Borrower with Id = {request.BorrowerId} is not found.");

            if (await repository.ExistsAsync(l => l.BookId == request.BookId && l.ReturnDate == null))
                return GenericResult<LoanResponse>.Failure("This book is already loaned and not returned yet.");

            var loan = mapper.Map<Loan>(request);
            await repository.CreateAsync(loan);

            logger.LogInformation("Book {BookId} loaned to borrower {BorrowerId}", loan.BookId, loan.BorrowerId);
            return await GetByIdAsync(loan.Id);
        }

        public async Task<GenericResult<LoanResponse>> UpdateAsync(UpdateLoanRequest request)
        {
            if (request.ReturnDate is not null && request.ReturnDate < request.LoanDate)
                return GenericResult<LoanResponse>.Failure("Return date can't be before the loan date.");

            var loan = await repository.GetByIdAsync(request.Id);
            if (loan is null)
                return GenericResult<LoanResponse>.NotFound($"Loan with Id = {request.Id} is not found.");

            mapper.Map(request, loan);
            await repository.UpdateAsync(loan);

            return await GetByIdAsync(loan.Id);
        }

        public async Task<GenericResult<LoanResponse>> ReturnBookAsync(int id)
        {
            var loan = await repository.GetByIdAsync(id);
            if (loan is null)
                return GenericResult<LoanResponse>.NotFound($"Loan with Id = {id} is not found.");

            if (loan.ReturnDate is not null)
                return GenericResult<LoanResponse>.Failure("This book is already returned.");

            loan.ReturnDate = DateTime.UtcNow;
            await repository.UpdateAsync(loan);

            logger.LogInformation("Loan {Id} returned", id);
            return await GetByIdAsync(id);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var deleted = await repository.DeleteAsync(id);
            if (!deleted)
                return Result.NotFound($"Loan with Id = {id} not found.");

            logger.LogInformation("Loan with Id = {Id} deleted (soft delete)", id);
            return Result.Success();
        }
    }
}