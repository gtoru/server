using Microsoft.EntityFrameworkCore;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> Users { get; set; }

        public DbSet<SessionModel> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasAlternateKey(m => m.EmailAddress);

            modelBuilder.Entity<SessionModel>()
                .HasAlternateKey(m => m.UserId);
        }
    }
}
