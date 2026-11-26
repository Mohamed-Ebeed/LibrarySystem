using LibrarySystem.Dtos.Author;
using LibrarySystem.Filters;
using LibrarySystem.Results;
using LibrarySystem.Services;
using LibrarySystem.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DEPI_Session4_WebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ExceptionHandlingFilter))]
    [ServiceFilter(typeof(ActionTimingFilter))]
    public class AuthorsController(IAuthorService service) : BaseApiController
    {
        /// <summary>
        /// Get all authors with pagination, search by name and filter by birth date.
        /// </summary>
        /// <example>GET /api/Authors?pageNumber=1&amp;pageSize=5&amp;search=Nag</example>
        [HttpGet]
        [ProducesResponseType(typeof(GenericResult<PaginatedList<AuthorResponse>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null, DateTime? bornFrom = null, DateTime? bornTo = null)
            => HandleResult(await service.GetAllAsync(pageNumber, pageSize, search, bornFrom, bornTo));

        /// <summary>
        /// Get one author by id with his books.
        /// </summary>
        [HttpGet("{id:int}")]
        [ServiceFilter(typeof(ActionExecutionLoggingFilter))]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
            => HandleResult(await service.GetByIdAsync(id));

        /// <summary>
        /// Add a new author.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CreateAuthorRequest request)
        {
            var result = await service.CreateAsync(request);
            if (result.IsFailure)
                return HandleResult(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result);
        }

        /// <summary>
        /// Update an author.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GenericResult<AuthorResponse>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(UpdateAuthorRequest request)
            => HandleResult(await service.UpdateAsync(request));

        /// <summary>
        /// Delete an author (soft delete). Not allowed if he has books.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
            => HandleResult(await service.DeleteAsync(id));
    }
}