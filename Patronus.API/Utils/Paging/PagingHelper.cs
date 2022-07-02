using Microsoft.EntityFrameworkCore;

namespace Patronus.API.Utils.Paging
{
    public static class PagingHelper
    {
        /// <summary>
        /// Filters of items according to a PagingModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items to filter</param>
        /// <param name="paging">The filtering parameters</param>
        /// <returns>An object containing the filtered list and the total items and pages</returns>
        public static PagedSearchResult<T> Page<T>(this IEnumerable<T> items, PagingModel paging)
        {
            Validate(paging);
            var pageSize = paging.PageSize ?? int.MaxValue;
            var list = items.ToList();
            var start = (paging.PageNumber - 1) * pageSize;
            var count = Math.Min(list.Count - start, pageSize);
            var paged = list.GetRange(start, count);
            return new PagedSearchResult<T>(paged, list.Count, pageSize, paging.PageNumber);
        }

        /// <summary>
        /// Modifies a queryable according to a PagingModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The query to filter</param>
        /// <param name="paging">The filtering parameters</param>
        /// <returns>An object containing the filtered results of the query and the total items and pages</returns>
        public static PagedSearchResult<T> Page<T>(this IQueryable<T> items, PagingModel paging)
        {
            Validate(paging);
            var total = items.Count();
            var result = Offset(items, paging, total, out int pageSize);
            var paged = result.ToList();
            return new PagedSearchResult<T>(paged, total, pageSize, paging.PageNumber);
        }

        /// <summary>
        /// Asynchronously modifies a queryable according to a PagingModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The query to filter</param>
        /// <param name="paging">The filtering parameters</param>
        /// <returns>An object containing the filtered results of the query and the total items and pages</returns>
        public static async Task<PagedSearchResult<T>> PageAsync<T>(this IQueryable<T> items, PagingModel paging)
        {
            Validate(paging);
            int pageSize;
            var total = await items.CountAsync();
            var result = Offset(items, paging, total, out pageSize);
            var paged = await result.ToListAsync();
            return new PagedSearchResult<T>(paged, total, pageSize, paging.PageNumber);
        }

        /// <summary>
        /// Asynchronously modifies a queryable according to a PagingModel, and converts it to another type
        /// </summary>
        /// <typeparam name="T">The type that is being paged</typeparam>
        /// <typeparam name="Y">The type to be converted to</typeparam>
        /// <param name="items">The query to filter</param>
        /// <param name="paging">The filtering parameters</param>
        /// <param name="conversionFunction">The function used to convert the objects</param>
        /// <returns>An object containing the converted, filtered results of the query and the total items and pages</returns>
        public static async Task<PagedSearchResult<Y>> PageAndConvertAsync<T, Y>(this IQueryable<T> items, PagingModel paging, Func<T, Y> conversionFunction)
        {
            Validate(paging);
            int pageSize;
            var total = await items.CountAsync();
            var result = Offset(items, paging, total, out pageSize);
            var paged = await result.ToListAsync();
            var converted = paged.Select(conversionFunction).ToList();
            return new PagedSearchResult<Y>(converted, total, pageSize, paging.PageNumber);
        }

        private static IQueryable<T> Offset<T>(IQueryable<T> items, PagingModel paging, int total, out int pageSize)
        {
            if (paging.PageSize != null)
            {
                pageSize = paging.PageSize.Value;
                var skip = (paging.PageNumber - 1) * pageSize;
                items = items.Skip(skip).Take(pageSize);
            }
            else
            {
                pageSize = total;
            }
            return items;
        }

        private static void Validate(PagingModel paging)
        {
            if (paging.PageNumber <= 0)
            {
                paging.PageNumber = 1;
            }
            if (paging.PageSize <= 0)
            {
                throw new InvalidOperationException("PageSize must be greater than 0");
            }
        }
    }
}
