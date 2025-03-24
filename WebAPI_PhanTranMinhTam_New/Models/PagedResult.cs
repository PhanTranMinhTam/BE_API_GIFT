namespace WebAPI_PhanTranMinhTam_New.Models
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public bool HasNextPage => PageNumber * PageSize < TotalRecords;
        public bool HasPreviousPage => PageNumber > 1;
    }
}
