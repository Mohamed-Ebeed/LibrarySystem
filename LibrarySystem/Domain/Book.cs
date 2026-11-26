namespace LibrarySystem.Domain
{
    public class Book : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public virtual Author Author { get; set; } = null!;

        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}