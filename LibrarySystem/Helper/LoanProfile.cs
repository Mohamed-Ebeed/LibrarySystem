using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Loan;

namespace LibrarySystem.Helper
{
    public class LoanProfile : Profile
    {
        public LoanProfile()
        {
            CreateMap<Loan, LoanResponse>()
                .ForMember(dest => dest.BookTitle, opt => opt.MapFrom(src => src.Book.Title))
                .ForMember(dest => dest.BorrowerName, opt => opt.MapFrom(src => src.Borrower.Name))
                .ForMember(dest => dest.IsReturned, opt => opt.MapFrom(src => src.ReturnDate != null));

            CreateMap<CreateLoanRequest, Loan>()
                .ForMember(dest => dest.LoanDate, opt => opt.MapFrom(src => src.LoanDate ?? DateTime.UtcNow));

            
            CreateMap<UpdateLoanRequest, Loan>()
                .ForMember(dest => dest.BookId, opt => opt.Ignore())
                .ForMember(dest => dest.BorrowerId, opt => opt.Ignore());
        }
    }
}