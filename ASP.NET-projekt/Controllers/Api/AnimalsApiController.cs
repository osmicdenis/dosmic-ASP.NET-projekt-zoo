using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public AnimalsApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AnimalDto>> GetAll([FromQuery] string? search, [FromQuery] DietType? diet, [FromQuery] int? enclosureId)
        {
            Console.WriteLine($"Animals API GetAll called. search={search}, diet={diet}, enclosureId={enclosureId}");

            var animals = _zooRepository.GetAllAnimals().AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var normalizedSearch = search.Trim();
                animals = animals.Where(animal =>
                    animal.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    animal.Species.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase));
            }

            if (diet.HasValue)
            {
                animals = animals.Where(animal => animal.Diet == diet.Value);
            }

            if (enclosureId.HasValue)
            {
                animals = animals.Where(animal => animal.EnclosureId == enclosureId.Value);
            }

            var result = animals
                .OrderBy(animal => animal.Name)
                .Select(ToDto)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public ActionResult<AnimalDto> GetById(int id)
        {
            Console.WriteLine($"Animals API GetById called. id={id}");

            var animal = _zooRepository.GetAnimalById(id);

            if (animal == null)
            {
                return NotFound();
            }

            return Ok(ToDto(animal));
        }

        [HttpPost]
        public ActionResult<AnimalDto> Create([FromBody] AnimalUpsertDto model)
        {
            Console.WriteLine("Animals API Create called.");

            ValidateAnimal(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            if (model.EnclosureId.HasValue && !_zooRepository.GetAllEnclosures().Any(enclosure => enclosure.Id == model.EnclosureId.Value))
            {
                ModelState.AddModelError(nameof(model.EnclosureId), "Selected enclosure does not exist.");
                return ValidationProblem(ModelState);
            }

            var animal = new Animal
            {
                Name = model.Name!.Trim(),
                Species = model.Species!.Trim(),
                DateOfBirth = model.DateOfBirth,
                DateOfArrival = model.DateOfArrival,
                Diet = model.Diet,
                EnclosureId = model.EnclosureId
            };

            _zooRepository.AddAnimal(animal);
            _zooRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = animal.Id }, ToDto(animal));
        }

        [HttpPut("{id:int}")]
        public ActionResult<AnimalDto> Update(int id, [FromBody] AnimalUpsertDto model)
        {
            Console.WriteLine($"Animals API Update called. id={id}");

            ValidateAnimal(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var animal = _zooRepository.GetAnimalById(id);

            if (animal == null)
            {
                return NotFound();
            }

            if (model.EnclosureId.HasValue && !_zooRepository.GetAllEnclosures().Any(enclosure => enclosure.Id == model.EnclosureId.Value))
            {
                ModelState.AddModelError(nameof(model.EnclosureId), "Selected enclosure does not exist.");
                return ValidationProblem(ModelState);
            }

            animal.Name = model.Name!.Trim();
            animal.Species = model.Species!.Trim();
            animal.DateOfBirth = model.DateOfBirth;
            animal.DateOfArrival = model.DateOfArrival;
            animal.Diet = model.Diet;
            animal.EnclosureId = model.EnclosureId;

            _zooRepository.UpdateAnimal(animal);
            _zooRepository.SaveChanges();

            return Ok(ToDto(animal));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            Console.WriteLine($"Animals API Delete called. id={id}");

            var animal = _zooRepository.GetAnimalById(id);

            if (animal == null)
            {
                return NotFound();
            }

            _zooRepository.DeleteAnimal(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private AnimalDto ToDto(Animal animal)
        {
            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(item => item.Animals.Any(candidate => candidate.Id == animal.Id));

            return new AnimalDto
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                DateOfBirth = animal.DateOfBirth,
                DateOfArrival = animal.DateOfArrival,
                Diet = animal.Diet,
                Enclosure = enclosure == null
                    ? null
                    : new EnclosureSummaryDto
                    {
                        Id = enclosure.Id,
                        Name = enclosure.Name,
                        Type = enclosure.Type
                    },
                MedicalRecordsCount = animal.MedicalRecords.Count,
                FeedingsCount = animal.Feedings.Count
            };
        }

        private void ValidateAnimal(AnimalUpsertDto model)
        {
            var now = DateTime.Now;

            if (model.DateOfBirth > now)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth cannot be in the future.");
            }

            if (model.DateOfArrival > now)
            {
                ModelState.AddModelError(nameof(model.DateOfArrival), "Date of arrival cannot be in the future.");
            }

            if (model.DateOfBirth > model.DateOfArrival)
            {
                ModelState.AddModelError(nameof(model.DateOfBirth), "Date of birth must be before or equal to date of arrival.");
            }
        }
    }
}