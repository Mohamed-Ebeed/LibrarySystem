using LibrarySystem.Controllers;
using LibrarySystem.Dtos.Borrower;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibrarySystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    [ServiceFilter(typeof(ActionTimingFilter))]
    public class BorrowersController(IBorrowerService service) : BaseApiController
    {
        /// <summary>
        /// Get all borrowers with pagination, search by name and filter by membership date.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(GenericResult<PaginatedList<BorrowerResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null, DateTime? memberSince = null)
            => HandleResult(await service.GetAllAsync(pageNumber, pageSize, search, memberSince));

        /// <summary>
        /// Get one borrower by id.
        /// </summary>
        [HttpGet("{id:int}")]
        [ServiceFilter(typeof(ActionExecutionLoggingFilter))]
        [ProducesResponseType(typeof(GenericResult<BorrowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<BorrowerResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
            => HandleResult(await service.GetByIdAsync(id));

        /// <summary>
        /// Add a new borrower.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GenericResult<BorrowerResponse>), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(CreateBorrowerRequest request)
        {
            var result = await service.CreateAsync(request);
            if (result.IsFailure)
                return HandleResult(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
        }

        /// <summary>
        /// Update a borrower.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(GenericResult<BorrowerResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<BorrowerResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateBorrowerRequest request)
            => HandleResult(await service.UpdateAsync(request));

        /// <summary>
        /// Delete a borrower (soft delete). Not allowed if he still has books.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
            => HandleResult(await service.DeleteAsync(id));
    }
}