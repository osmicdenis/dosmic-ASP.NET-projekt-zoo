using ASP.NET_projekt.Dtos;
using ASP.NET_projekt.Data;
using ASP.NET_projekt.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers.Api
{
    [ApiController]
    [Route("api/zoos")]
    public class ZoosApiController : ControllerBase
    {
        private readonly ZooDbContext _dbContext;

        public ZoosApiController(ZooDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ZooDto>> GetAll([FromQuery] string? search)
        {
            var normalizedSearch = search?.Trim();

            var zoos = _dbContext.Zoos
                .Include(zoo => zoo.Enclosures)
                .Where(zoo => string.IsNullOrWhiteSpace(normalizedSearch) ||
                              zoo.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                              zoo.Location.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(zoo => zoo.Name)
                .Select(ToDto)
                .ToList();

            return Ok(zoos);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ZooDto> GetById(int id)
        {
            var zoo = _dbContext.Zoos
                .Include(zoo => zoo.Enclosures)
                .FirstOrDefault(zoo => zoo.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            return Ok(ToDto(zoo));
        }

        [HttpPost]
        public ActionResult<ZooDto> Create([FromBody] ZooUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var zoo = new Zoo
            {
                Name = model.Name!.Trim(),
                Location = model.Location!.Trim()
            };

            _dbContext.Zoos.Add(zoo);
            _dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = zoo.Id }, ToDto(zoo));
        }

        [HttpPut("{id:int}")]
        public ActionResult<ZooDto> Update(int id, [FromBody] ZooUpsertDto model)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var zoo = _dbContext.Zoos
                .Include(zoo => zoo.Enclosures)
                .FirstOrDefault(zoo => zoo.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            zoo.Name = model.Name!.Trim();
            zoo.Location = model.Location!.Trim();

            _dbContext.SaveChanges();

            return Ok(ToDto(zoo));
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var zoo = _dbContext.Zoos.FirstOrDefault(zoo => zoo.Id == id);

            if (zoo == null)
            {
                return NotFound();
            }

            _dbContext.Zoos.Remove(zoo);
            _dbContext.SaveChanges();

            return NoContent();
        }

        private static ZooDto ToDto(Zoo zoo)
        {
            return new ZooDto
            {
                Id = zoo.Id,
                Name = zoo.Name,
                Location = zoo.Location,
                Enclosures = zoo.Enclosures
                    .OrderBy(enclosure => enclosure.Name)
                    .Select(enclosure => new LookupDto
                    {
                        Id = enclosure.Id,
                        Name = enclosure.Name
                    })
                    .ToList()
            };
        }
    }
}