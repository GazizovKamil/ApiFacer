using ApiFacer.Classes;
using Microsoft.EntityFrameworkCore;

namespace ApiFacer.DB
{
    public class ApiDB : DbContext
    {
        public ApiDB(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Logins> Logins { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<People> People { get; set; }
        public DbSet<UserImages> UserImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, name = "Admin" },
                new Roles { Id = 2, name = "Фотограф" },
                new Roles { Id = 3, name = "Гость" }
            );
            modelBuilder.Entity<Users>().HasData(
               new Users { Id = 1, login = "admin", password = "admin", surname = "Артюхин", first_name = "Арсений", last_name ="А", id_role = 1 }
           );
        }
    }
}
