using LibrarySystem.Domain;
using LibrarySystem.Dtos.Borrower;
using LibrarySystem.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BorrowersController : ControllerBase
    {
        private readonly LibraryDbContext _dbContext;

        public BorrowersController(LibraryDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var borrowers = await _dbContext.Borrowers
                .Include(b => b.Loans)
                .ToListAsync();

            return Ok(borrowers.Select(ToResponse).ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var borrower = await _dbContext.Borrowers
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (borrower is null)
                return NotFound($"Borrower with Id = {id} is not found.");

            return Ok(ToResponse(borrower));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBorrowerRequest request)
        {
            var borrower = new Borrower
            {
                Name = request.Name,
                MembershipDate = request.MembershipDate ?? DateTime.Today
            };

            await _dbContext.Borrowers.AddAsync(borrower);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = borrower.Id }, ToResponse(borrower));
        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdateBorrowerRequest request)
        {
            var borrower = await _dbContext.Borrowers
                .Include(b => b.Loans)
                .FirstOrDefaultAsync(b => b.Id == request.Id);

            if (borrower is null)
                return NotFound($"Borrower with Id = {request.Id} is not found.");

            borrower.Name = request.Name;
            borrower.MembershipDate = request.MembershipDate;

            await _dbContext.SaveChangesAsync();

            return Ok(ToResponse(borrower));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var borrower = await _dbContext.Borrowers.FirstOrDefaultAsync(b => b.Id == id);
            if (borrower is null)
                return NotFound($"Borrower with Id = {id} is not found.");

            if (await _dbContext.Loans.AnyAsync(l => l.BorrowerId == id && l.ReturnDate == null))
                return BadRequest("Can't delete this borrower, he still has books not returned.");

            _dbContext.Borrowers.Remove(borrower);
            await _dbContext.SaveChangesAsync();

            return Ok("Borrower deleted successfully!");
        }

        private static BorrowerResponse ToResponse(Borrower borrower) => new BorrowerResponse
        {
            Id = borrower.Id,
            Name = borrower.Name,
            MembershipDate = borrower.MembershipDate,
            ActiveLoansCount = borrower.Loans.Count(l => l.ReturnDate == null)
        };
    }
}