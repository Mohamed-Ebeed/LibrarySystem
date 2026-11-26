using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Borrower;
using LibrarySystem.Helper;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public class BorrowerService(
        IGenericRepository<Borrower> repository,
        IGenericRepository<Loan> loanRepository,
        IMapper mapper,
        ILogger<BorrowerService> logger) : IBorrowerService
    {
        public async Task<GenericResult<PaginatedList<BorrowerResponse>>> GetAllAsync(int pageNumber, int pageSize, string? search, DateTime? memberSince)
        {
            var borrowers = await repository.GetPagedAsync(pageNumber, pageSize,
                b => (string.IsNullOrEmpty(search) || b.Name.Contains(search))
                  && (memberSince == null || b.MembershipDate >= memberSince),
                b => b.Loans);

            logger.LogInformation("Got {Count} borrowers (page {Page})", borrowers.TotalRecords, borrowers.PageNumber);
            return GenericResult<PaginatedList<BorrowerResponse>>.Success(borrowers.MapTo<Borrower, BorrowerResponse>(mapper));
        }

        public async Task<GenericResult<BorrowerResponse>> GetByIdAsync(int id)
        {
            var borrower = await repository.GetByIdAsync(id, b => b.Loans);
            if (borrower is null)
            {
                logger.LogWarning("Borrower with Id = {Id} is not found", id);
                return GenericResult<BorrowerResponse>.NotFound($"Borrower with Id = {id} is not found.");
            }

            return GenericResult<BorrowerResponse>.Success(mapper.Map<BorrowerResponse>(borrower));
        }

        public async Task<GenericResult<BorrowerResponse>> CreateAsync(CreateBorrowerRequest request)
        {
            var borrower = mapper.Map<Borrower>(request);
            await repository.CreateAsync(borrower);

            logger.LogInformation("Borrower {Name} created with Id = {Id}", borrower.Name, borrower.Id);
            return GenericResult<BorrowerResponse>.Success(mapper.Map<BorrowerResponse>(borrower));
        }

        public async Task<GenericResult<BorrowerResponse>> UpdateAsync(UpdateBorrowerRequest request)
        {
            var borrower = await repository.GetByIdAsync(request.Id);
            if (borrower is null)
                return GenericResult<BorrowerResponse>.NotFound($"Borrower with Id = {request.Id} is not found.");

            mapper.Map(request, borrower);
            await repository.UpdateAsync(borrower);

            return await GetByIdAsync(borrower.Id);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (!await repository.ExistsAsync(b => b.Id == id))
                return Result.NotFound($"Borrower with Id = {id} is not found.");

            // لازم يرجّع كتبه الأول
            if (await loanRepository.ExistsAsync(l => l.BorrowerId == id && l.ReturnDate == null))
                return Result.Failure("Can't delete this borrower, he still has books not returned.");

            await repository.DeleteAsync(id);
            logger.LogInformation("Borrower with Id = {Id} deleted (soft delete)", id);
            return Result.Success();
        }
    }
}