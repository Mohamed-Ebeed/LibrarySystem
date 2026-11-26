using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Author;

namespace LibrarySystem.Helper
{
    public class AuthorProfile : Profile
    {
        public AuthorProfile()
        {
            CreateMap<Author, AuthorResponse>()
                .ForMember(dest => dest.BooksCount, opt => opt.MapFrom(src => src.Books.Count))
                .ForMember(dest => dest.BookTitles, opt => opt.MapFrom(src => src.Books.Select(b => b.Title)));

            CreateMap<CreateAuthorRequest, Author>();
            CreateMap<UpdateAuthorRequest, Author>();
        }
    }
}
