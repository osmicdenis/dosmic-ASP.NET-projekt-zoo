using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/veterinarians")]
    public class VeterinariansApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public VeterinariansApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<VeterinarianDto>> GetAll([FromQuery] string? search)
        {
            var normalizedSearch = search?.Trim();

            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();
            var veterinarians = _zooRepository.GetAllVeterinarians()
                .Where(veterinarian => string.IsNullOrWhiteSpace(normalizedSearch) ||
                                       $"{veterinarian.FirstName} {veterinarian.LastName}".Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(veterinarian => veterinarian.FirstName)
                .ThenBy(veterinarian => veterinarian.LastName)
                .Select(veterinarian => ToDto(veterinarian, medicalRecords))
                .ToList();

            return Ok(veterinarians);
        }

        [HttpGet("{id:int}")]
        public ActionResult<VeterinarianDto> GetById(int id)
        {
            var veterinarian = _zooRepository.GetVeterinarianById(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();
            return Ok(ToDto(veterinarian, medicalRecords));
        }

        [HttpPost]
        public ActionResult<VeterinarianDto> Create([FromBody] VeterinarianUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var veterinarian = new Veterinarian
            {
                FirstName = model.FirstName!.Trim(),
                LastName = model.LastName!.Trim()
            };

            _zooRepository.AddVeterinarian(veterinarian);
            _zooRepository.SaveChanges();

            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();
            return CreatedAtAction(nameof(GetById), new { id = veterinarian.Id }, ToDto(veterinarian, medicalRecords));
        }

        [HttpPut("{id:int}")]
        public ActionResult<VeterinarianDto> Update(int id, [FromBody] VeterinarianUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var veterinarian = _zooRepository.GetVeterinarianById(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            veterinarian.FirstName = model.FirstName!.Trim();
            veterinarian.LastName = model.LastName!.Trim();

            _zooRepository.UpdateVeterinarian(veterinarian);
            _zooRepository.SaveChanges();

            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();
            return Ok(ToDto(veterinarian, medicalRecords));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var veterinarian = _zooRepository.GetVeterinarianById(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            _zooRepository.DeleteVeterinarian(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private static VeterinarianDto ToDto(Veterinarian veterinarian, List<MedicalRecord> medicalRecords)
        {
            var recordsForVeterinarian = medicalRecords.Where(record => record.VeterinarianId == veterinarian.Id).ToList();
            var treatedAnimalsCount = recordsForVeterinarian.Select(record => record.AnimalId).Distinct().Count();

            return new VeterinarianDto
            {
                Id = veterinarian.Id,
                FirstName = veterinarian.FirstName,
                LastName = veterinarian.LastName,
                MedicalRecordsCount = recordsForVeterinarian.Count,
                TreatedAnimalsCount = treatedAnimalsCount
            };
        }
    }
}