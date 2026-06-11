using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/feedings")]
    public class FeedingsApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public FeedingsApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FeedingDto>> GetAll([FromQuery] string? search, [FromQuery] int? animalId, [FromQuery] int? foodId)
        {
            var normalizedSearch = search?.Trim();

            var feedings = _zooRepository.GetAllFeedings()
                .Where(feeding =>
                    (!animalId.HasValue || feeding.AnimalId == animalId.Value) &&
                    (!foodId.HasValue || feeding.FoodId == foodId.Value))
                .Where(feeding =>
                    string.IsNullOrWhiteSpace(normalizedSearch) ||
                    feeding.Animal.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    feeding.Animal.Species.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                    feeding.Food.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(feeding => feeding.FeedingTime)
                .Select(ToDto)
                .ToList();

            return Ok(feedings);
        }

        [HttpGet("{id:int}")]
        public ActionResult<FeedingDto> GetById(int id)
        {
            var feeding = _zooRepository.GetFeedingById(id);

            if (feeding == null)
            {
                return NotFound();
            }

            return Ok(ToDto(feeding));
        }

        [HttpPost]
        public ActionResult<FeedingDto> Create([FromBody] FeedingUpsertDto model)
        {
            ValidateFeeding(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var feeding = new Feeding
            {
                AnimalId = model.AnimalId,
                FoodId = model.FoodId,
                FeedingTime = model.FeedingTime
            };

            _zooRepository.AddFeeding(feeding);
            _zooRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = feeding.Id }, ToDto(feeding));
        }

        [HttpPut("{id:int}")]
        public ActionResult<FeedingDto> Update(int id, [FromBody] FeedingUpsertDto model)
        {
            ValidateFeeding(model);

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var feeding = _zooRepository.GetFeedingById(id);

            if (feeding == null)
            {
                return NotFound();
            }

            feeding.AnimalId = model.AnimalId;
            feeding.FoodId = model.FoodId;
            feeding.FeedingTime = model.FeedingTime;

            _zooRepository.UpdateFeeding(feeding);
            _zooRepository.SaveChanges();

            return Ok(ToDto(feeding));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var feeding = _zooRepository.GetFeedingById(id);

            if (feeding == null)
            {
                return NotFound();
            }

            _zooRepository.DeleteFeeding(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private void ValidateFeeding(FeedingUpsertDto model)
        {
            var animal = _zooRepository.GetAnimalById(model.AnimalId);
            if (animal == null)
            {
                ModelState.AddModelError(nameof(model.AnimalId), "Selected animal does not exist.");
            }

            var foodExists = _zooRepository.GetAllFoods().Any(food => food.Id == model.FoodId);
            if (!foodExists)
            {
                ModelState.AddModelError(nameof(model.FoodId), "Selected food does not exist.");
            }

            if (animal != null && model.FeedingTime < animal.DateOfArrival)
            {
                ModelState.AddModelError(nameof(model.FeedingTime), "Feeding time cannot be before the animal's arrival date.");
            }
        }

        private FeedingDto ToDto(Feeding feeding)
        {
            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(item => item.Animals.Any(candidate => candidate.Id == feeding.AnimalId));

            return new FeedingDto
            {
                Id = feeding.Id,
                Animal = new AnimalSummaryDto
                {
                    Id = feeding.Animal.Id,
                    Name = feeding.Animal.Name,
                    Species = feeding.Animal.Species
                },
                Food = new FoodSummaryDto
                {
                    Id = feeding.Food.Id,
                    Name = feeding.Food.Name
                },
                Enclosure = enclosure == null
                    ? null
                    : new EnclosureSummaryDto
                    {
                        Id = enclosure.Id,
                        Name = enclosure.Name,
                        Type = enclosure.Type
                    },
                FeedingTime = feeding.FeedingTime
            };
        }
    }
}