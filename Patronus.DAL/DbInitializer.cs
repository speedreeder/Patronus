using Patronus.DAL.Entities;

namespace Patronus.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(PatronusContext context)
        {
            context.Contacts.Add(new Contact
            {
                Name = "Big Bird",
                Email = "big.bird@sesame.net",
                Phone = "9876543210",
                Line1 = "123 Sesame St",
                City = "New York",
                State = "NY",
                ZipCode = "10001"
            });

            context.Contacts.Add(new Contact
            {
                Name = "Kermit Frog",
                Email = "kermit.frog@sesame.net",
                Phone = "4444444444",
                Line1 = "123 Sesame St",
                City = "New York",
                State = "NY",
                ZipCode = "10001"
            });

            context.Contacts.Add(new Contact
            {
                Name = "Miss Piggy",
                Email = "miss.piggy@sesame.net",
                Phone = "5555555555",
                Line1 = "123 Sesame St",
                City = "New York",
                State = "NY",
                ZipCode = "10001"
            });

            context.SaveChanges();
        }
    }
}
