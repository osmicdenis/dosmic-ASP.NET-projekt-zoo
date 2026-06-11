using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class VeterinariansApiTests : ApiIntegrationTestBase
    {
        public VeterinariansApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededVeterinarians()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/veterinarians");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var veterinarians = await response.Content.ReadFromJsonAsync<VeterinarianDto[]>();
            veterinarians.Should().NotBeNull();
            veterinarians!.Should().Contain(veterinarian => veterinarian.Id == 1 && veterinarian.FirstName == "Dr. Zdravko");
        }

        [Fact]
        public async Task GetById_ReturnsVeterinarian_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/veterinarians/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<VeterinarianDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.FirstName.Should().Be("Dr. Zdravko");
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/veterinarians/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedVeterinarian_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/veterinarians", new VeterinarianUpsertDto
            {
                FirstName = "Maja",
                LastName = "Ivić"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<VeterinarianDto>();
            dto.Should().NotBeNull();
            dto!.FirstName.Should().Be("Maja");
            dto.LastName.Should().Be("Ivić");
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenPayloadIsInvalid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/veterinarians", new VeterinarianUpsertDto
            {
                FirstName = "A",
                LastName = "B"
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedVeterinarian_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/veterinarians/1", new VeterinarianUpsertDto
            {
                FirstName = "Updated",
                LastName = "Vet"
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<VeterinarianDto>();
            dto.Should().NotBeNull();
            dto!.FirstName.Should().Be("Updated");
            dto.LastName.Should().Be("Vet");
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/veterinarians/999999", new VeterinarianUpsertDto
            {
                FirstName = "Updated",
                LastName = "Vet"
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenVeterinarianExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/veterinarians", new VeterinarianUpsertDto
            {
                FirstName = "Delete",
                LastName = "Me"
            });
            var created = await createResponse.Content.ReadFromJsonAsync<VeterinarianDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/veterinarians/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
