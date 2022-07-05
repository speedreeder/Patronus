using Patronus.Api.Models;
using Refit;

namespace Patronus.API.Client
{
    public interface IPatronusApi
    {
        [Get("/api/contact/search")]
        Task<PagedSearchResult<ContactDto>> GetContactsAsync([Query] ContactSearchDto search);

        [Post("/api/contact")]
        Task<ContactDto> CreateContactAsync([Body] ContactDto contactDto);

        [Put("/api/contact")]
        Task<ContactDto> UpdateContactAsync([Body] ContactDto contactDto);

        [Delete("/api/contact/{contactId}")]
        Task<string> DeleteContactAsync(int contactId);
    }
}