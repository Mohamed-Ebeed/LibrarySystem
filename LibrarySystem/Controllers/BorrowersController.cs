using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Borrower;
using LibrarySystem.Helper;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowersController(LibraryDbContext dbContext, IMapper mapper, ILogger<BorrowersController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all borrowers with pagination, search by name and filter by membership date.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null, DateTime? memberSince = null)
        {
            var query = dbContext.Borrowers.Include(b => b.Loans).AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(b => b.Name.Contains(search));

            if (memberSince is not null)
                query = query.Where(b => b.MembershipDate >= memberSince);

            var page = await query.OrderBy(b => b.Id).ToPaginatedListAsync(pageNumber, pageSize);
            logger.LogInformation("Got {Count} borrowers (page {Page})", page.TotalRecords, page.PageNumber);

            var response = page.MapTo<Borrower, BorrowerResponse>(mapper);
            return Ok(GenericResult<PaginatedList<BorrowerResponse>>.Success(response));
        }

        /// <summary>
        /// Get one borrower by id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var borrower = await dbContext.Borrowers.Include(b => b.Loans).FirstOrDefaultAsync(b => b.Id == id);
            if (borrower is null)
            {
                logger.LogWarning("Borrower with Id = {Id} is not found", id);
                return NotFound(GenericResult<BorrowerResponse>.NotFound($"Borrower with Id = {id} is not found."));
            }

            return Ok(GenericResult<BorrowerResponse>.Success(mapper.Map<BorrowerResponse>(borrower)));
        }

        /// <summary>
        /// Add a new borrower.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateBorrowerRequest request)
        {
            var borrower = mapper.Map<Borrower>(request);
            await dbContext.Borrowers.AddAsync(borrower);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Borrower {Name} created with Id = {Id}", borrower.Name, borrower.Id);
            var result = GenericResult<BorrowerResponse>.Success(mapper.Map<BorrowerResponse>(borrower));
            return CreatedAtAction(nameof(GetById), new { id = borrower.Id }, result);
        }

        /// <summary>
        /// Update a borrower.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateBorrowerRequest request)
        {
            var borrower = await dbContext.Borrowers.Include(b => b.Loans).FirstOrDefaultAsync(b => b.Id == request.Id);
            if (borrower is null)
                return NotFound(GenericResult<BorrowerResponse>.NotFound($"Borrower with Id = {request.Id} is not found."));

            mapper.Map(request, borrower);
            await dbContext.SaveChangesAsync();

            return Ok(GenericResult<BorrowerResponse>.Success(mapper.Map<BorrowerResponse>(borrower)));
        }

        /// <summary>
        /// Delete a borrower (not allowed if he still has books).
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var borrower = await dbContext.Borrowers.FirstOrDefaultAsync(b => b.Id == id);
            if (borrower is null)
                return NotFound(Result.NotFound($"Borrower with Id = {id} is not found."));

            if (await dbContext.Loans.AnyAsync(l => l.BorrowerId == id && l.ReturnDate == null))
                return BadRequest(Result.Failure("Can't delete this borrower, he still has books not returned."));

            dbContext.Borrowers.Remove(borrower);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Borrower with Id = {Id} deleted", id);
            return Ok(Result.Success());
        }
    }
}