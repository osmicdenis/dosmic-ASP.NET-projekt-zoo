using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/medicalrecords")]
    public class MedicalRecordsApiController : ControllerBase
    {
        private readonly ZooDbContext _dbContext;

        public MedicalRecordsApiController(ZooDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<MedicalRecordDto>> GetAll([FromQuery] string? search, [FromQuery] int? animalId, [FromQuery] int? veterinarianId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var normalizedSearch = search?.Trim();

            var records = _dbContext.MedicalRecords
                .Include(record => record.Animal)
                .Include(record => record.Veterinarian)
                .Where(record =>
                    (!animalId.HasValue || record.AnimalId == animalId.Value) &&
                    (!veterinarianId.HasValue || record.VeterinarianId == veterinarianId.Value) &&
                    (!fromDate.HasValue || record.ExaminationDate >= fromDate.Value) &&
                    (!toDate.HasValue || record.ExaminationDate <= toDate.Value))
                .Where(record =>
                    string.IsNullOrWhiteSpace(normalizedSearch) ||
                    record.Diagnosis.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    record.Therapy.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    record.Animal.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    record.Veterinarian.FirstName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    record.Veterinarian.LastName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(record => record.ExaminationDate)
                .Select(ToDto)
                .ToList();

            return Ok(records);
        }

        [HttpGet("{id:int}")]
        public ActionResult<MedicalRecordDto> GetById(int id)
        {
            var record = _dbContext.MedicalRecords
                .Include(item => item.Animal)
                .Include(item => item.Veterinarian)
                .FirstOrDefault(item => item.Id == id);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(ToDto(record));
        }

        [HttpPost]
        public ActionResult<MedicalRecordDto> Create([FromBody] MedicalRecordUpsertDto model)
        {
            ValidateMedicalRecord(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var record = new MedicalRecord
            {
                Diagnosis = model.Diagnosis!.Trim(),
                Therapy = model.Therapy!.Trim(),
                ExaminationDate = model.ExaminationDate,
                AnimalId = model.AnimalId,
                VeterinarianId = model.VeterinarianId
            };

            _dbContext.MedicalRecords.Add(record);
            _dbContext.SaveChanges();

            var createdRecord = _dbContext.MedicalRecords
                .Include(item => item.Animal)
                .Include(item => item.Veterinarian)
                .First(item => item.Id == record.Id);

            return CreatedAtAction(nameof(GetById), new { id = createdRecord.Id }, ToDto(createdRecord));
        }

        [HttpPut("{id:int}")]
        public ActionResult<MedicalRecordDto> Update(int id, [FromBody] MedicalRecordUpsertDto model)
        {
            ValidateMedicalRecord(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var record = _dbContext.MedicalRecords
                .Include(item => item.Animal)
                .Include(item => item.Veterinarian)
                .FirstOrDefault(item => item.Id == id);

            if (record == null)
            {
                return NotFound();
            }

            record.Diagnosis = model.Diagnosis!.Trim();
            record.Therapy = model.Therapy!.Trim();
            record.ExaminationDate = model.ExaminationDate;
            record.AnimalId = model.AnimalId;
            record.VeterinarianId = model.VeterinarianId;

            _dbContext.SaveChanges();

            return Ok(ToDto(record));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var record = _dbContext.MedicalRecords.FirstOrDefault(item => item.Id == id);

            if (record == null)
            {
                return NotFound();
            }

            _dbContext.MedicalRecords.Remove(record);
            _dbContext.SaveChanges();

            return NoContent();
        }

        private void ValidateMedicalRecord(MedicalRecordUpsertDto model)
        {
            var animal = _dbContext.Animals.FirstOrDefault(item => item.Id == model.AnimalId);
            if (animal == null)
            {
                ModelState.AddModelError(nameof(model.AnimalId), "Selected animal does not exist.");
            }

            var veterinarianExists = _dbContext.Veterinarians.Any(veterinarian => veterinarian.Id == model.VeterinarianId);
            if (!veterinarianExists)
            {
                ModelState.AddModelError(nameof(model.VeterinarianId), "Selected veterinarian does not exist.");
            }
        }

        private MedicalRecordDto ToDto(MedicalRecord record)
        {
            var enclosure = _dbContext.Enclosures
                .Include(item => item.Zoo)
                .Include(item => item.Zookeeper)
                .Include(item => item.Animals)
                .FirstOrDefault(item => item.Animals.Any(candidate => candidate.Id == record.AnimalId));

            return new MedicalRecordDto
            {
                Id = record.Id,
                Diagnosis = record.Diagnosis,
                Therapy = record.Therapy,
                ExaminationDate = record.ExaminationDate,
                Animal = new AnimalSummaryDto
                {
                    Id = record.Animal.Id,
                    Name = record.Animal.Name,
                    Species = record.Animal.Species
                },
                Veterinarian = new VeterinarianSummaryDto
                {
                    Id = record.Veterinarian.Id,
                    FullName = $"{record.Veterinarian.FirstName} {record.Veterinarian.LastName}"
                },
                Enclosure = enclosure == null
                    ? null
                    : new EnclosureSummaryDto
                    {
                        Id = enclosure.Id,
                        Name = enclosure.Name,
                        Type = enclosure.Type
                    }
            };
        }
    }
}