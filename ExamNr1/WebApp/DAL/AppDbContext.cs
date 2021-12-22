using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApp.Domain;

namespace WebApp.DAL
{
    public class AppDbContext: DbContext
    {
        public DbSet<ExtraTopping> ExtraToppings { get; set; } = default!;
        public DbSet<Order> Orders { get; set; } = default!;
        public DbSet<OrderPizza> OrderPizzas { get; set; } = default!;
        public DbSet<Pizza> Pizzas { get; set; } = default!;
        public DbSet<PizzaToping> PizzaTopings { get; set; } = default!;
        public DbSet<Topping> Toppings { get; set; } = default!;
        
        private const string ConnectionString = "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_edvess_exam;MultipleActiveResultSets=true";

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