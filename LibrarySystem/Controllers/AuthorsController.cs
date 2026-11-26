using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Author;
using LibrarySystem.Helper;
using LibrarySystem.Infrastructure.Data;
using LibrarySystem.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibrarySystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController(LibraryDbContext dbContext, IMapper mapper, ILogger<AuthorsController> logger) : ControllerBase
    {
        /// <summary>
        /// Get all authors with pagination, search by name and filter by birth date.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, string? search = null, DateTime? bornFrom = null, DateTime? bornTo = null)
        {
            var query = dbContext.Authors.Include(a => a.Books).AsNoTracking().AsQueryable();

            // searching
            if (!string.IsNullOrEmpty(search))
                query = query.Where(a => a.Name.Contains(search));

            // filtration
            if (bornFrom is not null)
                query = query.Where(a => a.BirthDate >= bornFrom);
            if (bornTo is not null)
                query = query.Where(a => a.BirthDate <= bornTo);

            var page = await query.OrderBy(a => a.Id).ToPaginatedListAsync(pageNumber, pageSize);
            logger.LogInformation("Got {Count} authors (page {Page})", page.TotalRecords, page.PageNumber);

            var response = page.MapTo<Author, AuthorResponse>(mapper);
            return Ok(GenericResult<PaginatedList<AuthorResponse>>.Success(response));
        }

        /// <summary>
        /// Get one author by id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var author = await dbContext.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == id);
            if (author is null)
            {
                logger.LogWarning("Author with Id = {Id} is not found", id);
                return NotFound(GenericResult<AuthorResponse>.NotFound($"Author with Id = {id} is not found."));
            }

            return Ok(GenericResult<AuthorResponse>.Success(mapper.Map<AuthorResponse>(author)));
        }

        /// <summary>
        /// Add a new author.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(CreateAuthorRequest request)
        {
            if (request.BirthDate > DateTime.Today)
                return BadRequest(GenericResult<AuthorResponse>.Failure("Birth date can't be in the future."));

            var author = mapper.Map<Author>(request);
            await dbContext.Authors.AddAsync(author);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Author {Name} created with Id = {Id}", author.Name, author.Id);
            var result = GenericResult<AuthorResponse>.Success(mapper.Map<AuthorResponse>(author));
            return CreatedAtAction(nameof(GetById), new { id = author.Id }, result);
        }

        /// <summary>
        /// Update an author.
        /// </summary>
        [HttpPut]
        public async Task<IActionResult> Update(UpdateAuthorRequest request)
        {
            if (request.BirthDate > DateTime.Today)
                return BadRequest(GenericResult<AuthorResponse>.Failure("Birth date can't be in the future."));

            var author = await dbContext.Authors.Include(a => a.Books).FirstOrDefaultAsync(a => a.Id == request.Id);
            if (author is null)
                return NotFound(GenericResult<AuthorResponse>.NotFound($"Author with Id = {request.Id} is not found."));

            mapper.Map(request, author);
            await dbContext.SaveChangesAsync();

            return Ok(GenericResult<AuthorResponse>.Success(mapper.Map<AuthorResponse>(author)));
        }

        /// <summary>
        /// Delete an author (not allowed if he has books).
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            if (author is null)
                return NotFound(Result.NotFound($"Author with Id = {id} is not found."));

            if (await dbContext.Books.AnyAsync(b => b.AuthorId == id))
                return BadRequest(Result.Failure("Can't delete this author because he has books."));

            dbContext.Authors.Remove(author);
            await dbContext.SaveChangesAsync();

            logger.LogInformation("Author with Id = {Id} deleted", id);
            return Ok(Result.Success());
        }
    }
}