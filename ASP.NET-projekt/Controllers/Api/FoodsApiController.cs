using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/foods")]
    public class FoodsApiController : ControllerBase
    {
        private readonly IZooRepository _zooRepository;

        public FoodsApiController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FoodDto>> GetAll([FromQuery] string? search)
        {
            var normalizedSearch = search?.Trim();

            var foods = _zooRepository.GetAllFoods()
                .Where(food => string.IsNullOrWhiteSpace(normalizedSearch) ||
                               food.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(food => food.Name)
                .Select(ToDto)
                .ToList();

            return Ok(foods);
        }

        [HttpGet("{id:int}")]
        public ActionResult<FoodDto> GetById(int id)
        {
            var food = _zooRepository.GetFoodById(id);

            if (food == null)
            {
                return NotFound();
            }

            return Ok(ToDto(food));
        }

        [HttpPost]
        public ActionResult<FoodDto> Create([FromBody] FoodUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var food = new Food
            {
                Name = model.Name!.Trim()
            };

            _zooRepository.AddFood(food);
            _zooRepository.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = food.Id }, ToDto(food));
        }

        [HttpPut("{id:int}")]
        public ActionResult<FoodDto> Update(int id, [FromBody] FoodUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var food = _zooRepository.GetFoodById(id);

            if (food == null)
            {
                return NotFound();
            }

            food.Name = model.Name!.Trim();

            _zooRepository.UpdateFood(food);
            _zooRepository.SaveChanges();

            return Ok(ToDto(food));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var food = _zooRepository.GetFoodById(id);

            if (food == null)
            {
                return NotFound();
            }

            _zooRepository.DeleteFood(id);
            _zooRepository.SaveChanges();

            return NoContent();
        }

        private FoodDto ToDto(Food food)
        {
            return new FoodDto
            {
                Id = food.Id,
                Name = food.Name,
                FeedingsCount = food.Feedings.Count
            };
        }
    }
}