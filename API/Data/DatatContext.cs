using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DatatContext : DbContext
    {
        public DatatContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }
    }
}