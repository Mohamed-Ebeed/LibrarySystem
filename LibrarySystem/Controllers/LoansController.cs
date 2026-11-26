using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Loan;
using LibrarySystem.Helper;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController(LibraryDbContext dbContext, IMapper mapper, ILogger<LoansController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all loans with pagination, filter by book, borrower, or returned / not returned.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, int? bookId = null, int? borrowerId = null, bool? isReturned = null)
        {
            var query = dbContext.Loans.Include(l => l.Book).Include(l => l.Borrower).AsNoTracking().AsQueryable();

            if (bookId is not null)
                query = query.Where(l => l.BookId == bookId);
            if (borrowerId is not null)
                query = query.Where(l => l.BorrowerId == borrowerId);
            if (isReturned is not null)
                query = isReturned.Value
                    ? query.Where(l => l.ReturnDate != null)
                    : query.Where(l => l.ReturnDate == null);

            var page = await query.OrderBy(l => l.Id).ToPaginatedListAsync(pageNumber, pageSize);
            logger.LogInformation("Got {Count} loans (page {Page})", page.TotalRecords, page.PageNumber);

            var response = page.MapTo<Loan, LoanResponse>(mapper);
            return Ok(GenericResult<PaginatedList<LoanResponse>>.Success(response));
        }

        /// <summary>
        /// Get one loan by id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var loan = await GetLoanAsync(id);
            if (loan is null)
            {
                logger.LogWarning("Loan with Id = {Id} is not found", id);
                return NotFound(GenericResult<LoanResponse>.NotFound($"Loan with Id = {id} is not found."));
            }

            return Ok(GenericResult<LoanResponse>.Success(mapper.Map<LoanResponse>(loan)));
        }

        /// <summary>
        /// Loan a book to a borrower.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateLoanRequest request)
        {
            if (!await dbContext.Books.AnyAsync(b => b.Id == request.BookId))
                return BadRequest(GenericResult<LoanResponse>.Failure($"Book with Id = {request.BookId} is not found."));

            if (!await dbContext.Borrowers.AnyAsync(b => b.Id == request.BorrowerId))
                return BadRequest(GenericResult<LoanResponse>.Failure($"Borrower with Id = {request.BorrowerId} is not found."));

            if (await dbContext.Loans.AnyAsync(l => l.BookId == request.BookId && l.ReturnDate == null))
                return BadRequest(GenericResult<LoanResponse>.Failure("This book is already loaned and not returned yet."));

            var loan = mapper.Map<Loan>(request);
            await dbContext.Loans.AddAsync(loan);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Book {BookId} loaned to borrower {BorrowerId}", loan.BookId, loan.BorrowerId);
            var created = await GetLoanAsync(loan.Id);
            var result = GenericResult<LoanResponse>.Success(mapper.Map<LoanResponse>(created));
            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, result);
        }

        /// <summary>
        /// Update loan dates (send ReturnDate when the book comes back).
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateLoanRequest request)
        {
            if (request.ReturnDate is not null && request.ReturnDate < request.LoanDate)
                return BadRequest(GenericResult<LoanResponse>.Failure("Return date can't be before the loan date."));

            var loan = await GetLoanAsync(request.Id);
            if (loan is null)
                return NotFound(GenericResult<LoanResponse>.NotFound($"Loan with Id = {request.Id} is not found."));

            mapper.Map(request, loan);
            await dbContext.SaveChangesAsync();

            return Ok(GenericResult<LoanResponse>.Success(mapper.Map<LoanResponse>(loan)));
        }

        /// <summary>
        /// Delete a loan.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var loan = await dbContext.Loans.FirstOrDefaultAsync(l => l.Id == id);
            if (loan is null)
                return NotFound(Result.NotFound($"Loan with Id = {id} is not found."));

            dbContext.Loans.Remove(loan);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Loan with Id = {Id} deleted", id);
            return Ok(Result.Success());
        }

        private async Task<Loan?> GetLoanAsync(int id)
            => await dbContext.Loans.Include(l => l.Book).Include(l => l.Borrower).FirstOrDefaultAsync(l => l.Id == id);
    }
}