namespace Patronus.Api.Models
{
    public class ContactSearchDto : PagingModel
    {
        public int? ContactId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
