namespace LibrarySystem.Domain
{
    public class Author : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }

        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}