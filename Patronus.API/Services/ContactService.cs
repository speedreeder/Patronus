using Patronus.API.DTOs;
using Patronus.API.Utils.Paging;
using Patronus.DAL;
using Patronus.DAL.Entities;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Patronus.API.Services
{
    public interface IContactService
    {
        Task<(ContactDto ContactResult, string Message)> CreateContactAsync(ContactDto contactDto);
        Task<string> DeleteContactAsync(int contactId);
        Contact DtoToEntity(ContactDto dto);
        ContactDto EntityToDto(Contact entity);
        Task<PagedSearchResult<ContactDto>> SearchContactsAsync(ContactSearchDto searchDto);
        Task<(ContactDto ContactResult, string Message)> UpdateContactAsync(ContactDto contactDto);
    }

    public class ContactService : IContactService
    {
        private readonly PatronusContext _patronusContext;

        public ContactService(PatronusContext patronusContext)
        {
            _patronusContext = patronusContext;
        }


        public async Task<(ContactDto ContactResult, string Message)> CreateContactAsync(ContactDto contactDto)
        {
            var validationMessage = ValidateContact(contactDto);

            if (!string.IsNullOrEmpty(validationMessage))
            {
                return (null, validationMessage);
            }

            var entity = DtoToEntity(contactDto);

            await _patronusContext.Contacts.AddAsync(entity);
            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), string.Empty);
        }

        public async Task<(ContactDto ContactResult, string Message)> UpdateContactAsync(ContactDto contactDto)
        {
            var validationMessage = ValidateContact(contactDto);

            if (!string.IsNullOrEmpty(validationMessage))
            {
                return (null, validationMessage);
            }

            var entity = DtoToEntity(contactDto);

            // TODO: Catch not exists by Id
            _patronusContext.Contacts.Update(entity);
            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), string.Empty);
        }

        public async Task<string> DeleteContactAsync(int contactId)
        {
            var contact = await _patronusContext.Contacts.FindAsync(contactId);
            if (contact == null)
            {
                return $"Contact with {contactId} does not exist.";
            }

            _patronusContext.Remove(contact);
            await _patronusContext.SaveChangesAsync();
            return string.Empty;
        }

        public async Task<PagedSearchResult<ContactDto>> SearchContactsAsync(ContactSearchDto searchDto)
        {
            var query = _patronusContext.Contacts.AsQueryable();

            if (searchDto.ContactId.HasValue)
            {
                query.Where(c => c.ContactId == searchDto.ContactId);
            }

            if (!string.IsNullOrEmpty(searchDto.Email))
            {
                query.Where(c => c.Email == searchDto.Email);
            }

            if (!string.IsNullOrEmpty(searchDto.PhoneNumber))
            {
                query.Where(c => c.Phone == searchDto.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(searchDto.Name))
            {
                query.Where(c => c.Name == searchDto.Name);
            }

            return await query.PageAndConvertAsync(searchDto, EntityToDto);
        }

        private string ValidateContact(ContactDto contactDto)
        {
            if (!string.IsNullOrWhiteSpace(contactDto.Phone))
            {
                //source: https://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
                return Regex.IsMatch(contactDto.Phone, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$") ? string.Empty : $"Phone number {contactDto.Phone} is not properly formatted.";
            }

            if (!string.IsNullOrWhiteSpace(contactDto.Email))
            {
                // cheaper than regex? Not sure.
                try
                {
                    var email = new MailAddress(contactDto.Email);
                }
                catch (FormatException)
                {
                    return $"Email address {contactDto.Email} is not properly formatted.";
                }
            }

            if (!string.IsNullOrWhiteSpace(contactDto.Name))
            {
                return "Name is required.";
            }

            return string.Empty;
        }


        // I know there are libraries for these sort of POCO mappings (AutoMapper, etc)
        // but I've always prefered to do it by hand when possible.
        // More control at the cost of time spent.
        public ContactDto EntityToDto(Contact entity)
        {
            return new ContactDto
            {
                ContactId = entity.ContactId,
                Email = entity.Email,
                Name = entity.Name,
                Phone = entity.Phone,
                //Address = entity.
            };
        }

        public Contact DtoToEntity(ContactDto dto)
        {
            return new Contact
            {
                Email = dto.Email,
                Name = dto.Name,
                Phone = dto.Phone
            };
        }
    }

}
