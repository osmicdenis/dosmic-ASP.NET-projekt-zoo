using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/enclosures")]
    public class EnclosuresApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public EnclosuresApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<EnclosureDto>> GetAll([FromQuery] string? search, [FromQuery] int? zooId, [FromQuery] int? zookeeperId)
        {
            var normalizedSearch = search?.Trim();

            var enclosures = _zooRepository.GetAllEnclosures()
                .Where(enclosure =>
                    (string.IsNullOrWhiteSpace(normalizedSearch) ||
                     enclosure.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                     enclosure.Type.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                     enclosure.Zoo.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                     $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}".Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase)) &&
                    (!zooId.HasValue || enclosure.ZooId == zooId.Value) &&
                    (!zookeeperId.HasValue || enclosure.ZookeeperId == zookeeperId.Value))
                .OrderBy(enclosure => enclosure.Name)
                .Select(ToDto)
                .ToList();

            return Ok(enclosures);
        }

        [HttpGet("{id:int}")]
        public ActionResult<EnclosureDto> GetById(int id)
        {
            var enclosure = _zooRepository.GetEnclosureById(id);

            if (enclosure == null)
            {
                return NotFound();
            }

            return Ok(ToDto(enclosure));
        }

        [HttpPost]
        public ActionResult<EnclosureDto> Create([FromBody] EnclosureUpsertDto model)
        {
            ValidateEnclosure(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var enclosure = new Enclosure
            {
                Name = model.Name!.Trim(),
                Type = model.Type!.Trim(),
                Capacity = model.Capacity,
                ZooId = model.ZooId,
                ZookeeperId = model.ZookeeperId
            };

            _zooRepository.AddEnclosure(enclosure);
            _zooRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = enclosure.Id }, ToDto(enclosure));
        }

        [HttpPut("{id:int}")]
        public ActionResult<EnclosureDto> Update(int id, [FromBody] EnclosureUpsertDto model)
        {
            ValidateEnclosure(model, id);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var enclosure = _zooRepository.GetEnclosureById(id);

            if (enclosure == null)
            {
                return NotFound();
            }

            enclosure.Name = model.Name!.Trim();
            enclosure.Type = model.Type!.Trim();
            enclosure.Capacity = model.Capacity;
            enclosure.ZooId = model.ZooId;
            enclosure.ZookeeperId = model.ZookeeperId;

            _zooRepository.UpdateEnclosure(enclosure);
            _zooRepository.SaveChanges();

            return Ok(ToDto(enclosure));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var enclosure = _zooRepository.GetEnclosureById(id);

            if (enclosure == null)
            {
                return NotFound();
            }

            _zooRepository.DeleteEnclosure(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private void ValidateEnclosure(EnclosureUpsertDto model, int? existingEnclosureId = null)
        {
            var zooExists = _zooRepository.GetAllZoos().Any(zoo => zoo.Id == model.ZooId);
            if (!zooExists)
            {
                ModelState.AddModelError(nameof(model.ZooId), "Selected zoo does not exist.");
            }

            var zookeeperExists = _zooRepository.GetAllZookeepers().Any(zookeeper => zookeeper.Id == model.ZookeeperId);
            if (!zookeeperExists)
            {
                ModelState.AddModelError(nameof(model.ZookeeperId), "Selected zookeeper does not exist.");
            }

            if (existingEnclosureId.HasValue)
            {
                var existing = _zooRepository.GetEnclosureById(existingEnclosureId.Value);
                if (existing != null && model.Capacity < existing.Animals.Count)
                {
                    ModelState.AddModelError(nameof(model.Capacity), $"Capacity cannot be less than current animals count ({existing.Animals.Count}).");
                }
            }
        }

        private static EnclosureDto ToDto(Enclosure enclosure)
        {
            return new EnclosureDto
            {
                Id = enclosure.Id,
                Name = enclosure.Name,
                Type = enclosure.Type,
                Capacity = enclosure.Capacity,
                Zoo = new ZooSummaryDto
                {
                    Id = enclosure.Zoo.Id,
                    Name = enclosure.Zoo.Name,
                    Location = enclosure.Zoo.Location
                },
                Zookeeper = new ZookeeperSummaryDto
                {
                    Id = enclosure.Zookeeper.Id,
                    FullName = $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}"
                },
                Animals = enclosure.Animals
                    .OrderBy(animal => animal.Name)
                    .Select(animal => new AnimalSummaryDto
                    {
                        Id = animal.Id,
                        Name = animal.Name,
                        Species = animal.Species
                    })
                    .ToList()
            };
        }
    }
}