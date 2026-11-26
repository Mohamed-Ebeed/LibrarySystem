using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.Borrower
{
    public class UpdateBorrowerRequest
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public DateTime MembershipDate { get; set; }
    }
}