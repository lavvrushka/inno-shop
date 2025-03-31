namespace ProductManagement.Domain.Models
{
    public class Pagination<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
        public List<T> Items { get; private set; }
        public Pagination(List<T> items, int count, PageSettings pageSettings)
        {
            Items = items;
            TotalCount = count;
            PageSize = pageSettings.PageSize;
            CurrentPage = pageSettings.PageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSettings.PageSize);
        }
    }
}
