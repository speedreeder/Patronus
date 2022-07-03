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
        Task<(ContactDto ContactResult, List<string> Messages)> CreateContactAsync(ContactDto contactDto);
        Task<string> DeleteContactAsync(int contactId);
        Contact DtoToEntity(ContactDto dto);
        ContactDto EntityToDto(Contact entity);
        Task<PagedSearchResult<ContactDto>> SearchContactsAsync(ContactSearchDto searchDto);
        Task<(ContactDto ContactResult, List<string> Messages)> UpdateContactAsync(ContactDto contactDto);
    }

    public class ContactService : IContactService
    {
        private readonly PatronusContext _patronusContext;

        public ContactService(PatronusContext patronusContext)
        {
            _patronusContext = patronusContext;
        }


        public async Task<(ContactDto ContactResult, List<string> Messages)> CreateContactAsync(ContactDto contactDto)
        {
            var validationMessages = ValidateContact(contactDto);

            if (validationMessages.Any())
            {
                return (null, validationMessages);
            }

            var entity = DtoToEntity(contactDto);

            await _patronusContext.Contacts.AddAsync(entity);
            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), validationMessages);
        }

        public async Task<(ContactDto ContactResult, List<string> Messages)> UpdateContactAsync(ContactDto contactDto)
        {
            var validationMessages = ValidateContact(contactDto);

            if (validationMessages.Any())
            {
                return (null, validationMessages);
            }

            var entity = DtoToEntity(contactDto);

            // TODO: Catch not exists by Id
            _patronusContext.Contacts.Update(entity);
            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), validationMessages);
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

        private List<string> ValidateContact(ContactDto contactDto)
        {
            var errorMessages = new List<string>();
            if (!string.IsNullOrWhiteSpace(contactDto.Phone))
            {
                //source: https://stackoverflow.com/questions/16699007/regular-expression-to-match-standard-10-digit-phone-number
                if (!Regex.IsMatch(contactDto.Phone, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$")) errorMessages.Add($"Phone number {contactDto.Phone} is not properly formatted.");
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
                    errorMessages.Add($"Email address {contactDto.Email} is not properly formatted.");
                }
            }

            if (string.IsNullOrWhiteSpace(contactDto.Name))
            {
                errorMessages.Add("Name is required.");
            }

            return errorMessages;
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
                Address = new AddressDto
                {
                    Line1 = entity.Address.Line1,
                    Line2 = entity.Address.Line2,
                    City = entity.Address.City,
                    State = entity.Address.State,
                    ZipCode = entity.Address.ZipCode
                }
            };
        }

        public Contact DtoToEntity(ContactDto dto)
        {
            return new Contact
            {
                Email = dto.Email,
                Name = dto.Name,
                Phone = dto.Phone,
                Address = new Address
                {
                    Line1 = dto.Address.Line1,
                    Line2 = dto.Address.Line2,
                    City = dto.Address.City,
                    State = dto.Address.State,
                    ZipCode = dto.Address.ZipCode
                }
            };
        }
    }

}
