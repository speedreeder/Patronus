namespace Patronus.API.DTOs
{
    public class ContactDto
    {
        public int? ContactId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public AddressDto Address { get; set; }
    }
}
