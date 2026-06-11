using ASP.NET_projekt.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests
{
    public abstract class ApiIntegrationTestBase : IClassFixture<CustomWebApplicationFactory>
    {
        protected ApiIntegrationTestBase(CustomWebApplicationFactory factory)
        {
            Factory = factory;
        }

        protected CustomWebApplicationFactory Factory { get; }

        protected async Task ResetDatabaseAsync()
        {
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }

        protected HttpClient CreateClient()
        {
            return Factory.CreateClient();
        }
    }
}
