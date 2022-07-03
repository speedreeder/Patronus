using AutoFixture.Xunit2;
using FluentAssertions;
using Patronus.API.DTOs;
using Patronus.API.Services;
using Patronus.DAL;
using static Patronus.API.Test.Customizations;

namespace Patronus.API.Test
{
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
                Address = new AddressDto
                {
                    Line1 = "123 Sesame St",
                    Line2 = "APT 1",
                    State = "NY",
                    City = "Syracuse",
                    ZipCode = "13224"
                }
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

        [Fact]
        public async Task UpdateContactAsync_ShouldValidateInput(ContactService sut)
        {
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
                Address = new AddressDto
                {
                    Line1 = "123 Sesame St",
                    Line2 = "APT 1",
                    State = "NY",
                    City = "Syracuse",
                    ZipCode = "13224"
                }
            };
            var result = await sut.CreateContactAsync(contactDto);
            result.Messages.Should().BeNullOrEmpty();

            patronusContext.Contacts.Should().HaveCount(1);
            patronusContext.Contacts.First().Name.Should().Be(contactDto.Name);
            patronusContext.Contacts.First().Email.Should().Be(contactDto.Email);
            patronusContext.Contacts.First().Phone.Should().Be(contactDto.Phone);
            patronusContext.Contacts.First().Address.Line1.Should().Be(contactDto.Address.Line1);
            patronusContext.Contacts.First().Address.Line2.Should().Be(contactDto.Address.Line2);
            patronusContext.Contacts.First().Address.City.Should().Be(contactDto.Address.City);
            patronusContext.Contacts.First().Address.State.Should().Be(contactDto.Address.State);
            patronusContext.Contacts.First().Address.ZipCode.Should().Be(contactDto.Address.ZipCode);
        }

        [Fact]
        public void UpdateContactAsync_ShouldUpdateExistingEntity()
        {

        }

        [Fact]
        public void SearchContactAsync_ShouldSearchContext()
        {

        }

        [Fact]
        public void DeleteContactAsync_ShouldRemoveFromContext()
        {

        }
    }
}