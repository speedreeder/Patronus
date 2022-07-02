namespace Patronus.API.Utils.Paging
{
    public class PagedSearchResult<T>
    {
        public PagedSearchResult(List<T> data, int total, int pageSize, int page = 1)
        {
            Data = data;
            Total = total;
            PageSize = pageSize;
            Page = page;
            TotalPages = (int)Math.Ceiling(total / (double)pageSize);
        }

        public PagedSearchResult() : this(new List<T>(), 0, 0) { }

        /// <summary>
        /// The collection of data found in the search.
        /// </summary>
        public List<T> Data { get; set; }

        /// <summary>
        /// The total number of records found.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// The total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// The page requested.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// The number of records to take.
        /// </summary>
        public int PageSize { get; set; }
    }
}
