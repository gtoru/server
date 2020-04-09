using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using server.core.Domain;
using server.core.Domain.Authentication;
using server.core.Domain.Misc;
using server.core.Domain.Tasks;
using server.core.Domain.Tasks.Helpers;

namespace server.core.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VariantTask> Tasks { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<TestSession> TestSessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureUser(modelBuilder.Entity<User>());
            ConfigureQuiz(modelBuilder.Entity<Quiz>());
            ConfigureSessions(modelBuilder.Entity<TestSession>());
            ConfigureTask(modelBuilder.Entity<VariantTask>());
            ConfigureQuizTask(modelBuilder.Entity<QuizTask>());
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

            var converter = new ValueConverter<AccessLevel, string>(
                v => v.ToString(),
                v => Enum.Parse<AccessLevel>(v));
            entityTypeBuilder
                .Property(m => m.AccessLevel)
                .HasConversion(converter);

            entityTypeBuilder
                .Ignore(m => m.CurrentSession);

            entityTypeBuilder
                .HasMany(m => m.TestSessions)
                .WithOne(m => m.User)
                .HasForeignKey(m => m.UserId);
        }

        private void ConfigureQuiz(EntityTypeBuilder<Quiz> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasKey(m => m.QuizId);

            entityTypeBuilder
                .Property(m => m.Locked)
                .HasDefaultValue(false);
        }

        private void ConfigureTask(EntityTypeBuilder<VariantTask> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasKey(m => m.TaskId);

            entityTypeBuilder
                .Property(m => m.Variants)
                // ATTENTION: Postgres specific type. Will not work with other DB
                .HasColumnType("jsonb");

            entityTypeBuilder
                .Property(m => m.Locked)
                .HasDefaultValue(false);
        }

        private void ConfigureSessions(EntityTypeBuilder<TestSession> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasKey(m => m.SessionId);

            entityTypeBuilder
                .HasOne(m => m.Quiz)
                .WithMany();

            entityTypeBuilder
                .Property(m => m.Answers)
                // ATTENTION: Postgres specific type. Will not work with other DB
                .HasColumnType("jsonb");

            entityTypeBuilder
                .Property(m => m.Result)
                .HasDefaultValue(0);

            entityTypeBuilder
                .Property(m => m.IsFinished)
                .HasDefaultValue(false);

            entityTypeBuilder
                .Property(m => m.TimeProvider)
                .HasConversion(
                    t => "UTC",
                    t => new UtcTimeProvider());

            entityTypeBuilder
                .HasOne(m => m.User)
                .WithMany(m => m.TestSessions);
        }

        private void ConfigureQuizTask(EntityTypeBuilder<QuizTask> entityTypeBuilder)
        {
            entityTypeBuilder
                .HasKey(m => new {m.QuizId, m.TaskId});

            entityTypeBuilder
                .HasOne(m => m.Quiz)
                .WithMany(m => m.Tasks)
                .HasForeignKey(m => m.QuizId);

            entityTypeBuilder
                .HasOne(m => m.Task)
                .WithMany()
                .HasForeignKey(m => m.TaskId);
        }
    }
}
