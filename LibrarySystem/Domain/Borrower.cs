namespace LibrarySystem.Domain
{
    public class Borrower : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}