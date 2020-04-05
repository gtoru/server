using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using server.core.Domain;

namespace server.core.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder.Entity<User>());
        }

        private void ConfigureUser(EntityTypeBuilder<User> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasKey(m => m.UserId);

            entityTypeBuilder
                .OwnsOne(m => m.Email,
                    builder =>
                    {
                        builder.HasIndex(m => m.Address);
                        builder.Property(m => m.IsVerified);
                    });

            entityTypeBuilder
                .OwnsOne(m => m.PersonalInfo,
                    builder =>
                    {
                        builder.Property(m => m.Address);
                        builder.Property(m => m.Birthday);
                        builder.Property(m => m.Employer);
                        builder.Property(m => m.Name);
                        builder.Property(m => m.Occupation);
                    });

            entityTypeBuilder
                .OwnsOne(m => m.Password, builder =>
                {
                    builder.Property(m => m.HashedPassword);
                    builder.Property(m => m.HashAlgorithm);
                });
        }
    }
}
