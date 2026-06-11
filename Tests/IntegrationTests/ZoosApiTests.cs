using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class ZoosApiTests : ApiIntegrationTestBase
    {
        public ZoosApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededZoos()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zoos");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var zoos = await response.Content.ReadFromJsonAsync<ZooDto[]>();
            zoos.Should().NotBeNull();
            zoos!.Should().Contain(zoo => zoo.Id == 1 && zoo.Name == "Zagreb Zoo");
        }

        [Fact]
        public async Task GetById_ReturnsZoo_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zoos/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ZooDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.Name.Should().Be("Zagreb Zoo");
            dto.EnclosuresCount.Should().Be(3);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zoos/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedZoo_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var payload = new ZooUpsertDto
            {
                Name = "Osijek Zoo",
                Location = "Osijek, Croatia"
            };

            var response = await CreateClient().PostAsJsonAsync("/api/zoos", payload);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<ZooDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be(payload.Name);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
            db.Zoos.Any(zoo => zoo.Name == "Osijek Zoo").Should().BeTrue();
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenPayloadIsInvalid()
        {
            await ResetDatabaseAsync();

            var payload = new ZooUpsertDto
            {
                Name = "A",
                Location = "B"
            };

            var response = await CreateClient().PostAsJsonAsync("/api/zoos", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedZoo_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var payload = new ZooUpsertDto
            {
                Name = "Zagreb Zoo Updated",
                Location = "Zagreb, Croatia"
            };

            var response = await CreateClient().PutAsJsonAsync("/api/zoos/1", payload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ZooDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("Zagreb Zoo Updated");

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
            db.Zoos.Single(zoo => zoo.Id == 1).Name.Should().Be("Zagreb Zoo Updated");
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var payload = new ZooUpsertDto { Name = "Missing Zoo", Location = "Nowhere" };

            var response = await CreateClient().PutAsJsonAsync("/api/zoos/999999", payload);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenZooExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/zoos", new ZooUpsertDto
            {
                Name = "Temp Zoo",
                Location = "Temp City"
            });

            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await createResponse.Content.ReadFromJsonAsync<ZooDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/zoos/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
