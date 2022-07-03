using Patronus.API.DTOs;
using Patronus.API.Utils.Paging;
using Refit;

namespace Patronus.API.Client
{
    public interface IPatronusApi
    {
        [Get("/api/contact/search")]
        Task<PagedSearchResult<ContactDto>> GetContactsAsync(int contactId);
    }
}