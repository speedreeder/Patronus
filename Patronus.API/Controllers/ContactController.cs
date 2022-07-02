﻿using Microsoft.AspNetCore.Mvc;
using Patronus.API.DTOs;
using Patronus.API.Services;
using Patronus.API.Utils.Paging;

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

        [HttpGet("{contactId}")]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> GetContactAsync(int contactId)
        {
            var contacts = await _contactService.SearchContactsAsync(new ContactSearchDto
            {
                ContactId = contactId
            });

            return Ok(contacts);
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> GetContactsAsync(ContactSearchDto search)
        {
            var contacts = await _contactService.SearchContactsAsync(search);

            return Ok(contacts);
        }

        [HttpPost]
        public async Task<ActionResult<ContactDto>> CreateContactAsync(ContactDto contactDto)
        {
            var result = await _contactService.CreateContactAsync(contactDto);

            if(!string.IsNullOrWhiteSpace(result.Message))
            {
                return BadRequest(result.Message);
            }

            return Ok(result.ContactResult);
        }

        [HttpPut]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> UpdateContactAsync(ContactDto contactDto)
        {
            var result = await _contactService.UpdateContactAsync(contactDto);

            if (!string.IsNullOrWhiteSpace(result.Message))
            {
                return BadRequest(result.Message);
            }

            return Ok(result.ContactResult);
        }

        [HttpDelete("{contactId}")]
        public async Task<ActionResult<PagedSearchResult<ContactDto>>> UpdateContactAsync(int contactId)
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