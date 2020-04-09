using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using server.core.Infrastructure;

namespace Infrastructure.Tests.Repository
{
    [SetUpFixture]
    public class DbSetUpFixture
    {
        public const string PostgresUser = "gto-db-user";
        public const string PostgresPassword = "gto-db-password";
        public const string PostgresDb = "gto-db";
        private TestcontainersContainer _postgres;
        private ITestcontainersBuilder<TestcontainersContainer> _testcontainersBuilder;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("postgres:12.2")
                .WithName("gto-infra-test")
                .WithPortBinding(5432)
                .WithEnvironment("POSTGRES_USER", PostgresUser)
                .WithEnvironment("POSTGRES_PASSWORD", PostgresPassword)
                .WithEnvironment("POSTGRES_DB", PostgresDb)
                .WithCleanUp(true)
                .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432));
            _postgres = _testcontainersBuilder.Build();
            await _postgres.StartAsync();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _postgres.DisposeAsync();
        }

        public static async Task<AppDbContext> GetContextAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql($"Host={Environment.GetEnvironmentVariable("GTO_DB_HOST") ?? "localhost"};" +
                           $"Database={PostgresDb};" +
                           $"Username={PostgresUser};" +
                           $"Password={PostgresPassword}")
                .Options;

            var context = new AppDbContext(options);
            await context.Database.MigrateAsync();

            return context;
        }
    }
}
