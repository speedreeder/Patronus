using Microsoft.AspNetCore.Mvc;
using Patronus.Api.Models;
using Patronus.API.Services;

namespace Patronus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;

        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> GetContactsAsync([FromQuery]ContactSearchDto search)
        {
            var contacts = await _contactService.SearchContactsAsync(search);

            return Ok(contacts);
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContactAsync(ContactDto contactDto)
        {
            var result = await _contactService.CreateContactAsync(contactDto);

            if(result.Messages.Any())
            {
                return BadRequest(string.Join("; ", result.Messages.Select(m => m.ErrorMessage)));
            }

            return Ok(result.ContactResult);
        }

        [HttpPut]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> UpdateContactAsync(ContactDto contactDto)
        {
            if (!contactDto.ContactId.HasValue || contactDto.ContactId == 0)
            {
                return BadRequest("ContactId cannot be null or 0.");
            }

            var result = await _contactService.UpdateContactAsync(contactDto);

            if (result.Messages.Any())
            {
                return BadRequest(string.Join("; ", result.Messages.Select(m => m.ErrorMessage)));
            }

            return Ok(result.ContactResult);
        }

        [HttpDelete("{contactId:int}")]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> DeleteContactAsync(int contactId)
        {
            var result = await _contactService.DeleteContactAsync(contactId);

            if (!string.IsNullOrWhiteSpace(result))
            {
                return BadRequest(result);
            }

            return Ok();
        }
    }
}
