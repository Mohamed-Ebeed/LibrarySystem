using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.Borrower
{
    public class CreateBorrowerRequest
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public DateTime? MembershipDate { get; set; }
    }
}
