using AutoMapper;
using LibrarySystem.Domain;
using LibrarySystem.Dtos.Borrower;

namespace LibrarySystem.Helper
{
    public class BorrowerProfile : Profile
    {
        public BorrowerProfile()
        {
            CreateMap<Borrower, BorrowerResponse>()
                .ForMember(dest => dest.ActiveLoansCount,
                    opt => opt.MapFrom(src => src.Loans.Count(l => l.ReturnDate == null)));

            CreateMap<CreateBorrowerRequest, Borrower>()
                .ForMember(dest => dest.MembershipDate,
                    opt => opt.MapFrom(src => src.MembershipDate ?? DateTime.Today));

            CreateMap<UpdateBorrowerRequest, Borrower>();
        }
    }
}