using Microsoft.EntityFrameworkCore;
using Patronus.DAL.Entities;

namespace Patronus.DAL
{
    public class PatronusContext : DbContext
    {
        public PatronusContext(DbContextOptions<PatronusContext> options) : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }

    }
}