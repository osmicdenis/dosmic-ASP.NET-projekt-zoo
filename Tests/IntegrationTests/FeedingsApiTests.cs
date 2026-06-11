using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class FeedingsApiTests : ApiIntegrationTestBase
    {
        public FeedingsApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededFeedings()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/feedings");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var feedings = await response.Content.ReadFromJsonAsync<FeedingDto[]>();
            feedings.Should().NotBeNull();
            feedings!.Should().Contain(feeding => feeding.Id == 1);
        }

        [Fact]
        public async Task GetById_ReturnsFeeding_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/feedings/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<FeedingDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.Animal.Id.Should().Be(1);
            dto.Food.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/feedings/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedFeeding_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/feedings", new FeedingUpsertDto
            {
                AnimalId = 1,
                FoodId = 2,
                FeedingTime = new DateTime(2024, 5, 4, 11, 0, 0)
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<FeedingDto>();
            dto.Should().NotBeNull();
            dto!.Animal.Id.Should().Be(1);
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenFeedingTimeIsBeforeArrival()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/feedings", new FeedingUpsertDto
            {
                AnimalId = 1,
                FoodId = 1,
                FeedingTime = new DateTime(2010, 1, 1)
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedFeeding_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/feedings/1", new FeedingUpsertDto
            {
                AnimalId = 1,
                FoodId = 2,
                FeedingTime = new DateTime(2024, 5, 4, 12, 0, 0)
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<FeedingDto>();
            dto.Should().NotBeNull();
            dto!.Food.Id.Should().Be(2);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/feedings/999999", new FeedingUpsertDto
            {
                AnimalId = 1,
                FoodId = 1,
                FeedingTime = new DateTime(2024, 5, 4, 11, 0, 0)
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFeedingExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/feedings", new FeedingUpsertDto
            {
                AnimalId = 1,
                FoodId = 2,
                FeedingTime = new DateTime(2024, 5, 4, 11, 0, 0)
            });
            var created = await createResponse.Content.ReadFromJsonAsync<FeedingDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/feedings/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
