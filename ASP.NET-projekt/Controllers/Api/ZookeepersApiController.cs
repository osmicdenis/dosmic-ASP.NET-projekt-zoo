using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/zookeepers")]
    public class ZookeepersApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public ZookeepersApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ZookeeperDto>> GetAll([FromQuery] string? search)
        {
            var normalizedSearch = search?.Trim();

            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            var zookeepers = _zooRepository.GetAllZookeepers()
                .Where(zookeeper => string.IsNullOrWhiteSpace(normalizedSearch) ||
                                    $"{zookeeper.FirstName} {zookeeper.LastName}".Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(zookeeper => zookeeper.FirstName)
                .ThenBy(zookeeper => zookeeper.LastName)
                .Select(zookeeper => ToDto(zookeeper, enclosures))
                .ToList();

            return Ok(zookeepers);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ZookeeperDto> GetById(int id)
        {
            var zookeeper = _zooRepository.GetZookeeperById(id);

            if (zookeeper == null)
            {
                return NotFound();
            }

            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            return Ok(ToDto(zookeeper, enclosures));
        }

        [HttpPost]
        public ActionResult<ZookeeperDto> Create([FromBody] ZookeeperUpsertDto model)
        {
            ValidateZookeeper(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var zookeeper = new Zookeeper
            {
                FirstName = model.FirstName!.Trim(),
                LastName = model.LastName!.Trim(),
                YearsOfExperience = model.YearsOfExperience,
                DateOfEmployment = model.DateOfEmployment
            };

            _zooRepository.AddZookeeper(zookeeper);
            _zooRepository.SaveChanges();

            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            return CreatedAtAction(nameof(GetById), new { id = zookeeper.Id }, ToDto(zookeeper, enclosures));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ZookeeperDto> Update(int id, [FromBody] ZookeeperUpsertDto model)
        {
            ValidateZookeeper(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var zookeeper = _zooRepository.GetZookeeperById(id);

            if (zookeeper == null)
            {
                return NotFound();
            }

            zookeeper.FirstName = model.FirstName!.Trim();
            zookeeper.LastName = model.LastName!.Trim();
            zookeeper.YearsOfExperience = model.YearsOfExperience;
            zookeeper.DateOfEmployment = model.DateOfEmployment;

            _zooRepository.UpdateZookeeper(zookeeper);
            _zooRepository.SaveChanges();

            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            return Ok(ToDto(zookeeper, enclosures));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var zookeeper = _zooRepository.GetZookeeperById(id);

            if (zookeeper == null)
            {
                return NotFound();
            }

            var assignedEnclosuresCount = _zooRepository.GetAllEnclosures().Count(enclosure => enclosure.ZookeeperId == id);
            if (assignedEnclosuresCount > 0)
            {
                return Conflict(new { message = "This zookeeper cannot be deleted because they are assigned to one or more enclosures." });
            }

            _zooRepository.DeleteZookeeper(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private void ValidateZookeeper(ZookeeperUpsertDto model)
        {
            if (model.DateOfEmployment > DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.DateOfEmployment), "Date of employment cannot be in the future.");
            }
        }

        private static ZookeeperDto ToDto(Zookeeper zookeeper, List<Enclosure> allEnclosures)
        {
            var assignedEnclosures = allEnclosures.Where(enclosure => enclosure.ZookeeperId == zookeeper.Id).ToList();
            var assignedAnimalsCount = assignedEnclosures.SelectMany(enclosure => enclosure.Animals).DistinctBy(animal => animal.Id).Count();

            return new ZookeeperDto
            {
                Id = zookeeper.Id,
                FirstName = zookeeper.FirstName,
                LastName = zookeeper.LastName,
                YearsOfExperience = zookeeper.YearsOfExperience,
                DateOfEmployment = zookeeper.DateOfEmployment,
                EnclosuresCount = assignedEnclosures.Count,
                AnimalsCount = assignedAnimalsCount
            };
        }
    }
}