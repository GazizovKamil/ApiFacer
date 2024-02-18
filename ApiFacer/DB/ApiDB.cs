using ApiFacer.Classes;
using ConsoleFaiserScript.Classes;
using ConsoleFaiserScript.DB;
using Microsoft.EntityFrameworkCore;

namespace ApiFacer.DB
{
    public class ApiDB : DbContext
    {
        public ApiDB(DbContextOptions options) : base(options) { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Events> Events { get; set; }
        public DbSet<Logins> Logins { get; set; }
        //public DbSet<Images> Images { get; set; }
        public DbSet<UserImages> UserImages { get; set; }
        //public DbSet<People> People { get; set; }
        public DbSet<PersonVk> PersonVk { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>().HasData(
                new Roles { Id = 1, name = "Admin" },
                new Roles { Id = 2, name = "Фотограф" }
            );
            modelBuilder.Entity<Users>().HasData(
               new Users { Id = 1, login = "root", password = "rimezaaa", surname = "Артюхин", first_name = "Арсений", last_name ="А", id_role = 1 }
           );
            modelBuilder.Entity<Events>().HasData(
               new Events { Id = 1, Name = "Future Games 2024", path = "FutureGames2024"},
               new Events { Id = 2, Name = "10 Future Games 2023", path = "10FutureGames2023"}
           );
        }
    }
}
