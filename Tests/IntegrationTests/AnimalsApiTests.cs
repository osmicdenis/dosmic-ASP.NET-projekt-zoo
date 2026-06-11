using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Dtos;
using System.Threading.Tasks;
using Xunit;
using System;
using System.Linq;
using System.Net;

namespace IntegrationTests
{
    public class AnimalsApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public AnimalsApiTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetAll_ReturnsSeededAnimals()
        {
            await ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            var animal = new Animal
            {
                Name = "Alpha",
                Species = "Lion",
                DateOfBirth = DateTime.Now.AddYears(-4),
                DateOfArrival = DateTime.Now.AddYears(-3),
                Diet = DietType.Carnivore
            };

            db.Animals.Add(animal);
            await db.SaveChangesAsync();

            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/animals");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var animals = await response.Content.ReadFromJsonAsync<AnimalDto[]>();
            animals.Should().NotBeNull();
            animals!.Should().ContainSingle(x => x.Id == animal.Id && x.Name == animal.Name);
        }

        [Fact]
        public async Task GetById_ReturnsAnimal_WhenExists()
        {
            await ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            var animal = new Animal
            {
                Name = "IntegrationTestAnimal",
                Species = "Spec",
                DateOfBirth = System.DateTime.Now.AddYears(-1),
                DateOfArrival = System.DateTime.Now.AddMonths(-6),
                Diet = DietType.Herbivore
            };

            db.Animals.Add(animal);
            await db.SaveChangesAsync();

            var client = _factory.CreateClient();
            var response = await client.GetAsync($"/api/animals/{animal.Id}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<AnimalDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(animal.Id);
            dto.Name.Should().Be(animal.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.GetAsync($"/api/animals/999999");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedAnimal_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var client = _factory.CreateClient();
            var payload = CreateValidUpsertDto();

            var response = await client.PostAsJsonAsync("/api/animals", payload);

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<AnimalDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be(payload.Name);
            dto.Species.Should().Be(payload.Species);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();
            db.Animals.Count().Should().Be(1);
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenDatesAreInvalid()
        {
            await ResetDatabaseAsync();

            var client = _factory.CreateClient();
            var payload = CreateValidUpsertDto();
            payload.DateOfBirth = DateTime.Now.AddDays(1);
            payload.DateOfArrival = DateTime.Now.AddDays(2);

            var response = await client.PostAsJsonAsync("/api/animals", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedAnimal_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            var animal = new Animal
            {
                Name = "Before",
                Species = "Bear",
                DateOfBirth = DateTime.Now.AddYears(-5),
                DateOfArrival = DateTime.Now.AddYears(-4),
                Diet = DietType.Omnivore
            };

            db.Animals.Add(animal);
            await db.SaveChangesAsync();

            var client = _factory.CreateClient();
            var payload = CreateValidUpsertDto();
            payload.Name = "After";
            payload.Species = "Polar Bear";

            var response = await client.PutAsJsonAsync($"/api/animals/{animal.Id}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<AnimalDto>();
            dto.Should().NotBeNull();
            dto!.Name.Should().Be("After");
            dto.Species.Should().Be("Polar Bear");

            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ZooDbContext>();
            var updatedAnimal = verifyDb.Animals.Single(x => x.Id == animal.Id);
            updatedAnimal.Name.Should().Be("After");
            updatedAnimal.Species.Should().Be("Polar Bear");
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var client = _factory.CreateClient();
            var payload = CreateValidUpsertDto();

            var response = await client.PutAsJsonAsync("/api/animals/999999", payload);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Put_ReturnsValidationProblem_WhenDatesAreInvalid()
        {
            await ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            var animal = new Animal
            {
                Name = "Before",
                Species = "Bear",
                DateOfBirth = DateTime.Now.AddYears(-5),
                DateOfArrival = DateTime.Now.AddYears(-4),
                Diet = DietType.Omnivore
            };

            db.Animals.Add(animal);
            await db.SaveChangesAsync();

            var client = _factory.CreateClient();
            var payload = CreateValidUpsertDto();
            payload.DateOfBirth = DateTime.Now.AddDays(2);

            var response = await client.PutAsJsonAsync($"/api/animals/{animal.Id}", payload);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenAnimalExists()
        {
            await ResetDatabaseAsync();

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            var animal = new Animal
            {
                Name = "ToDelete",
                Species = "Wolf",
                DateOfBirth = DateTime.Now.AddYears(-3),
                DateOfArrival = DateTime.Now.AddYears(-2),
                Diet = DietType.Carnivore
            };

            db.Animals.Add(animal);
            await db.SaveChangesAsync();

            var client = _factory.CreateClient();
            var response = await client.DeleteAsync($"/api/animals/{animal.Id}");

            response.StatusCode.Should().Be(HttpStatusCode.NoContent);

            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ZooDbContext>();
            verifyDb.Animals.Any(x => x.Id == animal.Id).Should().BeFalse();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var client = _factory.CreateClient();

            var response = await client.DeleteAsync("/api/animals/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private static AnimalUpsertDto CreateValidUpsertDto()
        {
            return new AnimalUpsertDto
            {
                Name = "Test Animal",
                Species = "Test Species",
                DateOfBirth = DateTime.Now.AddYears(-2),
                DateOfArrival = DateTime.Now.AddYears(-1),
                Diet = DietType.Herbivore,
                EnclosureId = null
            };
        }

        private async Task ResetDatabaseAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ZooDbContext>();

            db.Animals.RemoveRange(db.Animals);
            await db.SaveChangesAsync();
        }
    }
}
