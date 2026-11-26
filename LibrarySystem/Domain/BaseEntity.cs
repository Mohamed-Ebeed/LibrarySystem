namespace LibrarySystem.Domain
{
    public class BaseEntity
    {
        public int Id { get; set; }

        #region Audit
        public bool IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        #endregion
    }
}