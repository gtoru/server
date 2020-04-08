using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Modules;
using DotNet.Testcontainers.Containers.WaitStrategies;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Api.Tests
{
    [SetUpFixture]
    public class DbSetUpFixture
    {
        private const string PostgresUser = "gto-db-user";
        private const string PostgresPassword = "gto-db-password";
        private const string PostgresDb = "gto-db";
        private TestcontainersContainer _postgres;
        private ITestcontainersBuilder<TestcontainersContainer> _testcontainersBuilder;

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            _testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
                .WithImage("postgres:12.2")
                .WithName("gto-api-test")
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

        public static void ConfigureContext(DbContextOptionsBuilder builder)
        {
            builder.UseNpgsql($"Host={Environment.GetEnvironmentVariable("GTO_DB_HOST") ?? "localhost"};" +
                              $"Database={PostgresDb};" +
                              $"Username={PostgresUser};" +
                              $"Password={PostgresPassword}");
        }
    }
}
