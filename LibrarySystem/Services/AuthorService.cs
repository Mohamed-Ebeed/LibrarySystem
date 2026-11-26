using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Author;
using LibrarySystem.Helper;
using LibrarySystem.Repositories;
using LibrarySystem.Results;

namespace LibrarySystem.Services
{
    public class AuthorService(
        IGenericRepository<Author> repository,
        IGenericRepository<Book> bookRepository,
        IMapper mapper,
        ILogger<AuthorService> logger) : IAuthorService
    {
        public async Task<GenericResult<PaginatedList<AuthorResponse>>> GetAllAsync(int pageNumber, int pageSize, string? search, DateTime? bornFrom, DateTime? bornTo)
        {
            // search بالاسم + filter بتاريخ الميلاد
            var authors = await repository.GetPagedAsync(pageNumber, pageSize,
                a => (string.IsNullOrEmpty(search) || a.Name.Contains(search))
                  && (bornFrom == null || a.BirthDate >= bornFrom)
                  && (bornTo == null || a.BirthDate <= bornTo),
                a => a.Books);

            logger.LogInformation("Got {Count} authors (page {Page})", authors.TotalRecords, authors.PageNumber);
            return GenericResult<PaginatedList<AuthorResponse>>.Success(authors.MapTo<Author, AuthorResponse>(mapper));
        }

        public async Task<GenericResult<AuthorResponse>> GetByIdAsync(int id)
        {
            var author = await repository.GetByIdAsync(id, a => a.Books);
            if (author is null)
            {
                logger.LogWarning("Author with Id = {Id} is not found", id);
                return GenericResult<AuthorResponse>.NotFound($"Author with Id = {id} is not found.");
            }

            return GenericResult<AuthorResponse>.Success(mapper.Map<AuthorResponse>(author));
        }

        public async Task<GenericResult<AuthorResponse>> CreateAsync(CreateAuthorRequest request)
        {
            if (request.BirthDate > DateTime.Today)
                return GenericResult<AuthorResponse>.Failure("Birth date can't be in the future.");

            var author = mapper.Map<Author>(request);
            await repository.CreateAsync(author);

            logger.LogInformation("Author {Name} created with Id = {Id}", author.Name, author.Id);
            return GenericResult<AuthorResponse>.Success(mapper.Map<AuthorResponse>(author));
        }

        public async Task<GenericResult<AuthorResponse>> UpdateAsync(UpdateAuthorRequest request)
        {
            if (request.BirthDate > DateTime.Today)
                return GenericResult<AuthorResponse>.Failure("Birth date can't be in the future.");

            var author = await repository.GetByIdAsync(request.Id);
            if (author is null)
                return GenericResult<AuthorResponse>.NotFound($"Author with Id = {request.Id} is not found.");

            mapper.Map(request, author);
            await repository.UpdateAsync(author);

            return await GetByIdAsync(author.Id);
        }

        public async Task<Result> DeleteAsync(int id)
        {
            if (!await repository.ExistsAsync(a => a.Id == id))
                return Result.NotFound($"Author with Id = {id} is not found.");

            // مينفعش نسيب كتب من غير مؤلف
            if (await bookRepository.ExistsAsync(b => b.AuthorId == id))
                return Result.Failure("Can't delete this author because he has books.");

            await repository.DeleteAsync(id);
            logger.LogInformation("Author with Id = {Id} deleted (soft delete)", id);
            return Result.Success();
        }
    }
}