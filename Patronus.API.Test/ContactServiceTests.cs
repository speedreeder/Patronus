using AutoFixture.Xunit2;
using Patronus.API.DTOs;
using Patronus.API.Services;

namespace Patronus.API.Test
{
    public class ContactServiceTests
    {
        [Theory]
        [InlineAutoData("", "", "")]
        public async Task CreateContactAsync_ShouldValidateInput(
            string email,
            string name,
            string phone,
            ContactService sut)
        {
            var contactDto = new ContactDto
            {
                Email = email,
                Name = name,
                Phone = phone
            };

            var result = await sut.CreateContactAsync(contactDto);
        }

        [Fact]
        public void UpdateContactAsync_ShouldValidateInput()
        {

        }

        [Fact]
        public void CreateContactAsync_ShouldAddToContext()
        {

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