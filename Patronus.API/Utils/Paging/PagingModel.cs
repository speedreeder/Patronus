namespace Patronus.API.Utils.Paging
{
    public class PagingModel
    {
        public PagingModel()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public PagingModel(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public PagingModel(int pageNumber, int pageSize, string sortedBy)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public PagingModel(int pageNumber, int pageSize, string sortedBy, bool isSortAsc)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
