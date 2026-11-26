namespace LibrarySystem.Domain
{
    public class Loan : BaseEntity
    {
        public int BookId { get; set; }
        public virtual Book Book { get; set; } = null!;

        public int BorrowerId { get; set; }
        public virtual Borrower Borrower { get; set; } = null!;

        public DateTime LoanDate { get; set; }
        public DateTime? ReturnDate { get; set; }   // null => الكتاب لسه مع المستعير
    }
}