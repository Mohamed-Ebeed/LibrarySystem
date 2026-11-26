using System.ComponentModel.DataAnnotations;

namespace LibrarySystem.Dtos.Loan
{
    public class CreateLoanRequest
    {
        [Range(1, int.MaxValue)]
        public int BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int BorrowerId { get; set; }

      
        public DateTime? LoanDate { get; set; }
    }
}