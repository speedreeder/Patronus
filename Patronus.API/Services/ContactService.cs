using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Patronus.Api.Models;
using Patronus.API.Utils.Paging;
using Patronus.DAL;
using Patronus.DAL.Entities;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Patronus.API.Services
{
    public interface IContactService
    {
        Task<(ContactDto ContactResult, List<ValidationFailure> Messages)> CreateContactAsync(ContactDto contactDto);
        Task<string> DeleteContactAsync(int contactId);
        Contact DtoToEntity(ContactDto dto);
        ContactDto EntityToDto(Contact entity);
        Task<PagedSearchResult<ContactDto>> SearchContactsAsync(ContactSearchDto searchDto);
        Task<(ContactDto ContactResult, List<ValidationFailure> Messages)> UpdateContactAsync(ContactDto contactDto);
    }

    public class ContactService : IContactService
    {
        private readonly PatronusContext _patronusContext;
        private readonly IValidator<ContactDto> _validator;

        public ContactService(PatronusContext patronusContext, IValidator<ContactDto> validator)
        {
            _patronusContext = patronusContext;
            _validator = validator;
        }


        public async Task<(ContactDto ContactResult, List<ValidationFailure> Messages)> CreateContactAsync(ContactDto contactDto)
        {
            var validationResult = _validator.Validate(contactDto);

            if (!validationResult.IsValid)
            {
                return (null, validationResult.Errors);
            }

            var entity = DtoToEntity(contactDto);

            await _patronusContext.Contacts.AddAsync(entity);
            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), null);
        }

        public async Task<(ContactDto ContactResult, List<ValidationFailure> Messages)> UpdateContactAsync(ContactDto contactDto)
        {
            var validationResult = _validator.Validate(contactDto);

            if (!validationResult.IsValid)
            {
                return (null, validationResult.Errors);
            }

            var entity = await _patronusContext.Contacts.FindAsync(contactDto.ContactId);

            if (entity == null)
            {
                validationResult.Errors.Add(new ValidationFailure("ContactId", $"No Contact found with {contactDto.ContactId}", contactDto.ContactId));
                return (null, validationResult.Errors);
            }

            entity.Phone = contactDto.Phone;
            entity.Email = contactDto.Email;
            entity.Name = contactDto.Name;
            entity.Line1 = contactDto.Line1;
            entity.Line2 = contactDto.Line2;
            entity.City = contactDto.City;
            entity.State = contactDto.State;
            entity.ZipCode = contactDto.ZipCode;

            await _patronusContext.SaveChangesAsync();

            return (EntityToDto(entity), null);
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
                query = query.Where(c => c.ContactId == searchDto.ContactId);
            }

            if (!string.IsNullOrEmpty(searchDto.Email))
            {
                query = query.Where(c => c.Email == searchDto.Email);
            }

            if (!string.IsNullOrEmpty(searchDto.PhoneNumber))
            {
                query = query.Where(c => c.Phone == searchDto.PhoneNumber);
            }

            if (!string.IsNullOrEmpty(searchDto.Name))
            {
                query = query.Where(c => c.Name == searchDto.Name);
            }

            var x = query.ToList();

            var results = await query.PageAndConvertAsync(searchDto, EntityToDto);

            return results;
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
                Line1 = entity.Line1,
                Line2 = entity.Line2,
                City = entity.City,
                State = entity.State,
                ZipCode = entity.ZipCode
            };
        }

        public Contact DtoToEntity(ContactDto dto)
        {
            return new Contact
            {
                Email = dto.Email,
                Name = dto.Name,
                Phone = dto.Phone,
                Line1 = dto.Line1,
                Line2 = dto.Line2,
                City = dto.City,
                State = dto.State,
                ZipCode = dto.ZipCode
            };
        }
    }

}
