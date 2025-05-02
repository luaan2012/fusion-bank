namespace fusion.bank.core.Model
{
    public class PagedEventResult<T>
    {
        public List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalEvents { get; set; }
    }
}
