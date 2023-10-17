using ApiFacer.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApiFacer.DB
{
    public class ApiDB : DbContext
    {
        public ApiDB(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Events> Events { get; set; }
    }
}
