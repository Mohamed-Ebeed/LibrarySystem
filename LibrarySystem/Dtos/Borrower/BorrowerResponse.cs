namespace LibrarySystem.Dtos.Borrower
{
    public class BorrowerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime MembershipDate { get; set; }
        public int ActiveLoansCount { get; set; }
    }
}