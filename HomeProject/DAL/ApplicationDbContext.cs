using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Config> Configs { get; set; } = default!;
        public DbSet<ConfigShip> ConfigShips { get; set; } = default!;
        public DbSet<Ship> Ships { get; set; } = default!;
        public DbSet<Game> Games { get; set; } = default!;
        public DbSet<Replay> Replays { get; set; } = default!;

        private const string ConnectionString = "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_edvess;MultipleActiveResultSets=true";

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConnectionString);
        }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            foreach (var relationship in modelBuilder.Model
                .GetEntityTypes()
                .Where(e => !e.IsOwned())
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }
    }
}