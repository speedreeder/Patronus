using Patronus.API.Utils.Paging;

namespace Patronus.API.DTOs
{
    public class ContactSearchDto : PagingModel
    {
        public int? ContactId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
