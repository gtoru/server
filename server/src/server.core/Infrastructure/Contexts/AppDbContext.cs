using Microsoft.EntityFrameworkCore;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasAlternateKey(b => b.EmailAddress);
        }
    }
}
