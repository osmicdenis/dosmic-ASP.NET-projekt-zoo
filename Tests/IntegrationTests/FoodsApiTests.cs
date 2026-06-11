using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class FoodsApiTests : ApiIntegrationTestBase
    {
        public FoodsApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededFoods()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/foods");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var foods = await response.Content.ReadFromJsonAsync<FoodDto[]>();
            foods.Should().NotBeNull();
            foods!.Should().Contain(food => food.Id == 1 && food.Name == "Meat");
        }

        [Fact]
        public async Task GetById_ReturnsFood_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/foods/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<FoodDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.Name.Should().Be("Meat");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/foods/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedFood_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/foods", new FoodUpsertDto { Name = "Bamboo" });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<FoodDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("Bamboo");
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenPayloadIsInvalid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/foods", new FoodUpsertDto { Name = "A" });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedFood_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/foods/1", new FoodUpsertDto { Name = "Updated Meat" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<FoodDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("Updated Meat");
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/foods/999999", new FoodUpsertDto { Name = "Missing" });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenFoodExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/foods", new FoodUpsertDto { Name = "ToDelete" });
            var created = await createResponse.Content.ReadFromJsonAsync<FoodDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/foods/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
