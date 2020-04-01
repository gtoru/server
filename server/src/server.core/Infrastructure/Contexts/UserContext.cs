using Microsoft.EntityFrameworkCore;
using server.core.Infrastructure.Models;

namespace server.core.Infrastructure.Contexts
{
    public abstract class UserContext : DbContext
    {
        public DbSet<UserModel> Users { get; }

        protected abstract override void OnModelCreating(ModelBuilder modelBuilder);
    }
}
