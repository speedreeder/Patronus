using AutoFixture.Xunit2;
using FluentAssertions;
using Patronus.Api.Models;
using Patronus.API.Services;
using Patronus.DAL;
using Patronus.DAL.Entities;
using static Patronus.API.Test.Customizations;

namespace Patronus.API.Test
{
    // Not meant to be exhasutive, but a good sampling of tests.

    public class ContactServiceTests
    {
        [Theory]
        [AutoMoqInlineAutoData("goodemail@host", "test test", "5555555555", false)]
        [AutoMoqInlineAutoData("goodemail@", "test test", "5555555555", true)]
        [AutoMoqInlineAutoData("goodemail@host", "test test", "55aa5555bb5555", true)]
        [AutoMoqInlineAutoData("goodemail@host", "", "5555555555", true)]
        public async Task CreateContactAsync_ShouldValidateInput(
            string email,
            string name,
            string phone,
            bool resultHasErrorMessages,
            ContactService sut)
        {
            var contactDto = new ContactDto
            {
                Email = email,
                Name = name,
                Phone = phone,
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            var result = await sut.CreateContactAsync(contactDto);

            result.Should().NotBeNull();
            if (resultHasErrorMessages)
            {
                result.Messages.Should().NotBeNullOrEmpty();
            }
            else
            {
                result.Messages.Should().BeNullOrEmpty();
            }
        }

        [Theory]
        [AutoMoqInlineAutoData("goodemail@host", "test test", "5555555555", 1, false)]
        [AutoMoqInlineAutoData("goodemail@", "test test", "5555555555", 1, true)]
        [AutoMoqInlineAutoData("goodemail@host", "test test", "55aa5555bb5555", 1, true)]
        [AutoMoqInlineAutoData("goodemail@host", "", "5555555555", 1, true)]
        [AutoMoqInlineAutoData("goodemail@host", "", "5555555555", null, true)]
        public async Task UpdateContactAsync_ShouldValidateInput(
            string email,
            string name,
            string phone,
            int? contractId,
            bool resultHasErrorMessages,
            [Frozen] PatronusContext patronusContext,
            ContactService sut)
        {
            patronusContext.Add(new Contact
            {
                ContactId = 1,
                Name = "a a",
                Email = "newemail@test",
                Phone = "4444444444",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });
            patronusContext.SaveChanges();

            var contactDto = new ContactDto
            {
                ContactId = contractId,
                Email = email,
                Name = name,
                Phone = phone,
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            var result = await sut.UpdateContactAsync(contactDto);

            result.Should().NotBeNull();
            if (resultHasErrorMessages)
            {
                result.Messages.Should().NotBeNullOrEmpty();
            }
            else
            {
                result.Messages.Should().BeNullOrEmpty();
            }
        }

        [Theory, AutoMoqData]
        public async Task CreateContactAsync_ShouldAddToContext(
            [Frozen]PatronusContext patronusContext,
            ContactService sut)
        {
            var contactDto = new ContactDto
            {
                Email = "test@host",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };
            var result = await sut.CreateContactAsync(contactDto);
            result.Messages.Should().BeNullOrEmpty();

            patronusContext.Contacts.Should().HaveCount(1);
            patronusContext.Contacts.First().Name.Should().Be(contactDto.Name);
            patronusContext.Contacts.First().Email.Should().Be(contactDto.Email);
            patronusContext.Contacts.First().Phone.Should().Be(contactDto.Phone);
            patronusContext.Contacts.First().Line1.Should().Be(contactDto.Line1);
            patronusContext.Contacts.First().Line2.Should().Be(contactDto.Line2);
            patronusContext.Contacts.First().City.Should().Be(contactDto.City);
            patronusContext.Contacts.First().State.Should().Be(contactDto.State);
            patronusContext.Contacts.First().ZipCode.Should().Be(contactDto.ZipCode);
        }

        [Theory, AutoMoqData]
        public async Task UpdateContactAsync_ShouldUpdateExistingEntity(
            [Frozen] PatronusContext patronusContext,
            ContactService sut)
        {
            patronusContext.Add(new Contact
            {
                ContactId = 1,
                Name = "a a",
                Email = "newemail@test",
                Phone = "4444444444",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });
            patronusContext.SaveChanges();

            var contactDto = new ContactDto
            {
                ContactId = 1,
                Email = "freshemail@one",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            var result = await sut.UpdateContactAsync(contactDto);
            result.Messages.Should().BeNullOrEmpty();

            patronusContext.Contacts.Should().HaveCount(1);
            patronusContext.Contacts.First().Name.Should().Be(contactDto.Name);
            patronusContext.Contacts.First().Email.Should().Be(contactDto.Email);
            patronusContext.Contacts.First().Phone.Should().Be(contactDto.Phone);
            patronusContext.Contacts.First().Line1.Should().Be(contactDto.Line1);
            patronusContext.Contacts.First().Line2.Should().Be(contactDto.Line2);
            patronusContext.Contacts.First().City.Should().Be(contactDto.City);
            patronusContext.Contacts.First().State.Should().Be(contactDto.State);
            patronusContext.Contacts.First().ZipCode.Should().Be(contactDto.ZipCode);
        }

        [Theory]
        [AutoMoqInlineAutoData("contact@one", null, null, null, 1)]
        [AutoMoqInlineAutoData(null, "Contact Two", null, null, 2)]
        [AutoMoqInlineAutoData(null, null, "3333333333", null, 3)]
        [AutoMoqInlineAutoData(null, null, null, 4, 4)]
        public async Task SearchContactAsync_ShouldSearchContext(
            string email,
            string name,
            string phone,
            int? contactId,
            int? expectedId,
            [Frozen] PatronusContext patronusContext,
            ContactService sut)
        {
            patronusContext.Contacts.Add(new Contact
            {
                ContactId = 1,
                Name = "Contact One",
                Email = "contact@one",
                Phone = "1111111111",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });

            patronusContext.Contacts.Add(new Contact
            {
                ContactId = 2,
                Name = "Contact Two",
                Email = "contact@two",
                Phone = "2222222222",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });

            patronusContext.Contacts.Add(new Contact
            {
                ContactId = 3,
                Name = "Contact Three",
                Email = "contact@three",
                Phone = "3333333333",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });

            patronusContext.Contacts.Add(new Contact
            {
                ContactId = 4,
                Name = "Contact Four",
                Email = "contact@four",
                Phone = "4444444444",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });

            patronusContext.SaveChanges();

            var result = await sut.SearchContactsAsync(new ContactSearchDto
            {
                Email = email,
                PhoneNumber = phone,
                Name = name,
                ContactId = contactId
            });

            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            if (expectedId.HasValue)
            {
                result.Data.Single().ContactId.Should().Be(expectedId);
            }
            else
            {
                result.Data.Should().BeEmpty();
            }
        }

        [Theory]
        [AutoMoqInlineAutoData(1, 0)]
        [AutoMoqInlineAutoData(2, 1)]
        public async Task DeleteContactAsync_ShouldRemoveFromContext(
            int contactIdToDelete,
            int numberOfExpectedRemainingContacts,
            [Frozen] PatronusContext patronusContext,
            ContactService sut)
        {
            patronusContext.Contacts.Add(new Contact
            {
                ContactId = 1,
                Name = "Contact One",
                Email = "contact@one",
                Phone = "1111111111",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            });

            patronusContext.SaveChanges();

            await sut.DeleteContactAsync(contactIdToDelete);

            patronusContext.Contacts.Count().Should().Be(numberOfExpectedRemainingContacts);
        }
    }
}