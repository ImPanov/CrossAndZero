using Microsoft.EntityFrameworkCore;

namespace Packet.Shared
{
    public class ApplicationDbContext : DbContext
    {
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Game> Games { get; set; }
        public ApplicationDbContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("FileName=app.db");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User[] {
                new User {
                    UserId=1,
                    Password="Pa$$w0rd",
                    userName="bob"
                },
                new User {
                    UserId=2,
                    Password="Pa$$word",
                    userName="bab"
                },
            });
            modelBuilder.Entity<Game>().HasData(new Game[] { 
                new Game { 
                    GameId = Guid.NewGuid(),
                    Name="krut",
                    DateTimeGame = DateTime.Now,
                    Player1 = 1,
                    Player2 = 2,
                    Result1 = "win",
                    Result2 = "lose"
                },
            });
        }
    }
}
