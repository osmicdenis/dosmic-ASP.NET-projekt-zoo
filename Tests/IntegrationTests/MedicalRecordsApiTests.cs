using System.Net;
using System.Net.Http.Json;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Dtos;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class MedicalRecordsApiTests : ApiIntegrationTestBase
    {
        public MedicalRecordsApiTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetAll_ReturnsSeededMedicalRecords()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/medicalrecords");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var records = await response.Content.ReadFromJsonAsync<MedicalRecordDto[]>();
            records.Should().NotBeNull();
            records!.Should().Contain(record => record.Id == 1);
        }

        [Fact]
        public async Task GetById_ReturnsMedicalRecord_WhenExists()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/medicalrecords/1");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<MedicalRecordDto>();
            dto.Should().NotBeNull();
            dto!.Id.Should().Be(1);
            dto.Animal.Id.Should().Be(1);
            dto.Veterinarian.Id.Should().Be(1);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().GetAsync("/api/medicalrecords/999999");

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ReturnsCreatedMedicalRecord_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/medicalrecords", new MedicalRecordUpsertDto
            {
                Diagnosis = "Routine Check",
                Therapy = "Observation",
                ExaminationDate = new DateTime(2024, 5, 5),
                AnimalId = 1,
                VeterinarianId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var dto = await response.Content.ReadFromJsonAsync<MedicalRecordDto>();
            dto.Should().NotBeNull();
            dto!.Diagnosis.Should().Be("Routine Check");
        }

        [Fact]
        public async Task Post_ReturnsValidationProblem_WhenForeignKeysAreInvalid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PostAsJsonAsync("/api/medicalrecords", new MedicalRecordUpsertDto
            {
                Diagnosis = "Routine Check",
                Therapy = "Observation",
                ExaminationDate = new DateTime(2024, 5, 5),
                AnimalId = 999999,
                VeterinarianId = 999999
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_ReturnsUpdatedMedicalRecord_WhenPayloadIsValid()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/medicalrecords/1", new MedicalRecordUpsertDto
            {
                Diagnosis = "Updated Diagnosis",
                Therapy = "Updated Therapy",
                ExaminationDate = new DateTime(2024, 6, 1),
                AnimalId = 1,
                VeterinarianId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var dto = await response.Content.ReadFromJsonAsync<MedicalRecordDto>();
            dto.Should().NotBeNull();
            dto!.Diagnosis.Should().Be("Updated Diagnosis");
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenIdDoesNotExist()
        {
            await ResetDatabaseAsync();

            var response = await CreateClient().PutAsJsonAsync("/api/medicalrecords/999999", new MedicalRecordUpsertDto
            {
                Diagnosis = "Updated Diagnosis",
                Therapy = "Updated Therapy",
                ExaminationDate = new DateTime(2024, 6, 1),
                AnimalId = 1,
                VeterinarianId = 1
            });

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenMedicalRecordExists()
        {
            await ResetDatabaseAsync();

            var createResponse = await CreateClient().PostAsJsonAsync("/api/medicalrecords", new MedicalRecordUpsertDto
            {
                Diagnosis = "Temporary",
                Therapy = "Temporary",
                ExaminationDate = new DateTime(2024, 5, 5),
                AnimalId = 1,
                VeterinarianId = 1
            });
            var created = await createResponse.Content.ReadFromJsonAsync<MedicalRecordDto>();

            var deleteResponse = await CreateClient().DeleteAsync($"/api/medicalrecords/{created!.Id}");

            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}
