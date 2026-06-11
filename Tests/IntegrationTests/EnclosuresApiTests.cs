using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class EnclosuresApiTests : ApiIntegrationTestBase
    {
        public EnclosuresApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededEnclosures()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/enclosures");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var enclosures = await response.Content.ReadFromJsonAsync<EnclosureDto[]>();
            enclosures.Should().NotBeNull();
            enclosures!.Should().Contain(enclosure => enclosure.Id == 1 && enclosure.Name == "African Savanna");
        }

        [Fact]
        public async Task GetById_ReturnsEnclosure_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/enclosures/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<EnclosureDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.Name.Should().Be("African Savanna");
            dto.Zoo.Id.Should().Be(1);
            dto.Zookeeper.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/enclosures/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedEnclosure_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/enclosures", new EnclosureUpsertDto
            {
                Name = "Bird House",
                Type = "Aviary",
                Capacity = 20,
                ZooId = 1,
                ZookeeperId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<EnclosureDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("Bird House");
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenForeignKeysAreInvalid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/enclosures", new EnclosureUpsertDto
            {
                Name = "Bird House",
                Type = "Aviary",
                Capacity = 20,
                ZooId = 999999,
                ZookeeperId = 999999
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedEnclosure_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/enclosures/1", new EnclosureUpsertDto
            {
                Name = "African Savanna Updated",
                Type = "Grassland",
                Capacity = 60,
                ZooId = 1,
                ZookeeperId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<EnclosureDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("African Savanna Updated");
        }

        [Fact]
        public async Task Put_ReturnsValidationProblem_WhenCapacityIsTooLow()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/enclosures/1", new EnclosureUpsertDto
            {
                Name = "African Savanna",
                Type = "Grassland",
                Capacity = 1,
                ZooId = 1,
                ZookeeperId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/enclosures/999999", new EnclosureUpsertDto
            {
                Name = "Missing",
                Type = "Type",
                Capacity = 10,
                ZooId = 1,
                ZookeeperId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenEnclosureExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/enclosures", new EnclosureUpsertDto
            {
                Name = "Temp Enclosure",
                Type = "Temp",
                Capacity = 5,
                ZooId = 1,
                ZookeeperId = 1
            });
            var created = await createResponse.Content.ReadFromJsonAsync<EnclosureDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/enclosures/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
