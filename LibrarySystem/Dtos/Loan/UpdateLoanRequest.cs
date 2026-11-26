using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.Loan
{
    public class UpdateLoanRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime LoanDate { get; set; }

        
        public DateTime? ReturnDate { get; set; }
    }
}