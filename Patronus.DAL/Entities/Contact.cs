namespace Patronus.DAL.Entities
{
    public class Contact
    {
        public int ContactId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Address Address { get; set; }
    }
}