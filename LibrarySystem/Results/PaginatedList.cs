namespace LibrarySystem.Results
{
    public class PaginatedList<T>
    {
        private const int MaxPageSize = 20;
        private int _pageSize = 10;

        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : (value < 1 ? 10 : value);
        }
        public int TotalRecords { get; set; }
        public int TotalPage => (int)Math.Ceiling(TotalRecords / (double)PageSize);
        public IEnumerable<T> Data { get; set; } = new List<T>();
    }
}