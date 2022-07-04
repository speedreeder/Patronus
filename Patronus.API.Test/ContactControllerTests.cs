using AutoFixture.Xunit2;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Patronus.Api.Models;
using Patronus.API.Controllers;
using Patronus.API.Services;
using static Patronus.API.Test.Customizations;

namespace Patronus.API.Test
{

    public class ContactControllerTests
    {
        [Theory, AutoMoqData]
        public async Task CreateContactAsync_ShouldReturnBadRequestWithMessages(
            [Frozen] Mock<IContactService> mockContactService,
            ContactController sut)
        {
            var contactDto = new ContactDto
            {
                Email = "test@test",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            mockContactService.Setup(m => m.CreateContactAsync(contactDto)).ReturnsAsync((null, new List<ValidationFailure> { 
                new ValidationFailure("Email", "Message 1"), 
                new ValidationFailure("Phone", "Message 2") 
            }));

            var actionResult = await sut.CreateContactAsync(contactDto);

            var result = actionResult.Result as BadRequestObjectResult;
            result.Value.Should().NotBeNull();
            result.Value.Should().Be("Message 1; Message 2");
        }

        [Theory, AutoMoqData]
        public async Task CreateContactAsync_ShouldReturnOkWithResult(
            [Frozen] Mock<IContactService> mockContactService,
            ContactController sut)
        {
            var contactDto = new ContactDto
            {
                Email = "test@test",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            mockContactService.Setup(m => m.CreateContactAsync(contactDto)).ReturnsAsync((contactDto, null));

            var actionResult = await sut.CreateContactAsync(contactDto);
            var result = actionResult.Result as OkObjectResult;
            result.Value.Should().NotBeNull();
            var contactDtoResult = result.Value as ContactDto;
            contactDtoResult.Phone.Should().Be(contactDto.Phone);
            contactDtoResult.Email.Should().Be(contactDto.Email);
            contactDtoResult.Name.Should().Be(contactDto.Name);
        }

        [Theory, AutoMoqData]
        public async Task UpdateContactAsync_ShouldReturnBadRequestWithMessages(
            [Frozen] Mock<IContactService> mockContactService,
            ContactController sut)
        {
            var contactDto = new ContactDto
            {
                ContactId = 1,
                Email = "test@test",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            mockContactService.Setup(m => m.UpdateContactAsync(contactDto)).ReturnsAsync((null, new List<ValidationFailure> {
                new ValidationFailure("Email", "Message 1"),
                new ValidationFailure("Phone", "Message 2")
            }));

            var actionResult = await sut.UpdateContactAsync(contactDto);

            var result = actionResult.Result as BadRequestObjectResult;
            result.Value.Should().NotBeNull();
            result.Value.Should().Be("Message 1; Message 2");
        }

        [Theory, AutoMoqData]
        public async Task UpdateContactAsync_ShouldReturnOkWithResult(
            [Frozen] Mock<IContactService> mockContactService,
            ContactController sut)
        {
            var contactDto = new ContactDto
            {
                ContactId = 1,
                Email = "test@test",
                Name = "test test",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                Line2 = "APT 1",
                State = "NY",
                City = "Syracuse",
                ZipCode = "13224"
            };

            mockContactService.Setup(m => m.UpdateContactAsync(contactDto)).ReturnsAsync((contactDto, null));

            var actionResult = await sut.UpdateContactAsync(contactDto);
            var result = actionResult.Result as OkObjectResult;
            result.Value.Should().NotBeNull();
            var contactDtoResult = result.Value as ContactDto;
            contactDtoResult.Phone.Should().Be(contactDto.Phone);
            contactDtoResult.Email.Should().Be(contactDto.Email);
            contactDtoResult.Name.Should().Be(contactDto.Name);
        }
    }
}
