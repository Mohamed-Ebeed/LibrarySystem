
using LibrarySystem.Controllers;
using LibrarySystem.Dtos.Loan;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Session4_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    [ServiceFilter(typeof(ActionTimingFilter))]
    public class LoansController(ILoanService service) : BaseApiController
    {
        /// <summary>
        /// Get all loans with pagination, filter by book, borrower, or returned / not returned.
        /// </summary>
        /// <example>GET /api/Loans?borrowerId=1&amp;isReturned=false</example>
        [HttpGet]
        [ProducesResponseType(typeof(GenericResult<PaginatedList<LoanResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, int? bookId = null, int? borrowerId = null, bool? isReturned = null)
            => HandleResult(await service.GetAllAsync(pageNumber, pageSize, bookId, borrowerId, isReturned));

        /// <summary>
        /// Get one loan by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ServiceFilter(typeof(ActionExecutionLoggingFilter))]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
            => HandleResult(await service.GetByIdAsync(id));

        /// <summary>
        /// Loan a book to a borrower.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateLoanRequest request)
        {
            var result = await service.CreateAsync(request);
            if (result.IsFailure)
                return HandleResult(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
        }

        /// <summary>
        /// Update loan dates.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateLoanRequest request)
            => HandleResult(await service.UpdateAsync(request));

        /// <summary>
        /// Mark the book as returned (sets ReturnDate = now).
        /// </summary>
        [HttpPut("{id:int}/return")]
        [ProducesResponseType(typeof(GenericResult<LoanResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Return(int id)
            => HandleResult(await service.ReturnBookAsync(id));

        /// <summary>
        /// Delete a loan (soft delete).
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
            => HandleResult(await service.DeleteAsync(id));
    }
}
