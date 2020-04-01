using Microsoft.EntityFrameworkCore;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure.Contexts
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
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
