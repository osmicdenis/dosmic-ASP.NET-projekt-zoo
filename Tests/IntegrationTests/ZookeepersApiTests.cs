using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class ZookeepersApiTests : ApiIntegrationTestBase
    {
        public ZookeepersApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededZookeepers()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zookeepers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var zookeepers = await response.Content.ReadFromJsonAsync<ZookeeperDto[]>();
            zookeepers.Should().NotBeNull();
            zookeepers!.Should().Contain(zookeeper => zookeeper.Id == 1 && zookeeper.FirstName == "Marko");
        }

        [Fact]
        public async Task GetById_ReturnsZookeeper_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zookeepers/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ZookeeperDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.FirstName.Should().Be("Marko");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/zookeepers/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedZookeeper_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/zookeepers", new ZookeeperUpsertDto
            {
                FirstName = "Ivana",
                LastName = "Marić",
                YearsOfExperience = 3,
                DateOfEmployment = new DateTime(2020, 1, 1)
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<ZookeeperDto>();
            dto.Should().NotBeNull();
            dto!.FirstName.Should().Be("Ivana");
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenEmploymentDateIsInvalid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/zookeepers", new ZookeeperUpsertDto
            {
                FirstName = "Ivana",
                LastName = "Marić",
                YearsOfExperience = 3,
                DateOfEmployment = DateTime.Now.AddDays(1)
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedZookeeper_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/zookeepers/1", new ZookeeperUpsertDto
            {
                FirstName = "Marko",
                LastName = "Horvat",
                YearsOfExperience = 9,
                DateOfEmployment = new DateTime(2016, 5, 15)
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<ZookeeperDto>();
            dto.Should().NotBeNull();
            dto!.YearsOfExperience.Should().Be(9);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/zookeepers/999999", new ZookeeperUpsertDto
            {
                FirstName = "Marko",
                LastName = "Horvat",
                YearsOfExperience = 9,
                DateOfEmployment = new DateTime(2016, 5, 15)
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsConflict_WhenZookeeperHasEnclosures()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().DeleteAsync("/api/zookeepers/1");

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenZookeeperIsUnassigned()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/zookeepers", new ZookeeperUpsertDto
            {
                FirstName = "Delete",
                LastName = "Me",
                YearsOfExperience = 1,
                DateOfEmployment = new DateTime(2020, 1, 1)
            });
            var created = await createResponse.Content.ReadFromJsonAsync<ZookeeperDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/zookeepers/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
