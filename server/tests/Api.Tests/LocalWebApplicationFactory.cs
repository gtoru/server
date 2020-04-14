using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using server.core;
using server.core.Infrastructure;

namespace Api.Tests
{
    public class LocalWebApplicationFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.ConfigureServices(services =>
            {
                var dbContext = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (dbContext != null)
                    services.Remove(dbContext);

                services.AddDbContext<AppDbContext>(DbSetUpFixture.ConfigureContext);
            });
        }
    }
}
