using Microsoft.EntityFrameworkCore;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure.Contexts
{
    public class UserContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public UserContext(DbContextOptions<UserContext> options)
            : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasAlternateKey(b => b.EmailAddress);
        }
    }
}
