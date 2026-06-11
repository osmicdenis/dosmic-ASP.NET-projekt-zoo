using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers
{
    public class EnclosuresController : Controller
    {
        private readonly IZooRepository _zooRepository;

        public EnclosuresController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        public IActionResult Index()
        {
            return View(GetEnclosureListItems(null));
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            var enclosures = GetEnclosureListItems(query)
                .Select(enclosure => new
                {
                    id = enclosure.Id,
                    name = enclosure.Name,
                    type = enclosure.Type,
                    zooName = enclosure.ZooName,
                    capacity = enclosure.Capacity,
                    animalsCount = enclosure.AnimalsCount,
                    zookeeperName = enclosure.ZookeeperName,
                    occupancyStatusText = enclosure.OccupancyStatusText,
                    occupancyStatusClass = enclosure.OccupancyStatusClass,
                    occupancyDisplay = enclosure.OccupancyDisplay
                })
                .ToList();

            return Json(enclosures);
        }

        [HttpGet]
        public IActionResult SearchZoos(string? query)
        {
            var normalizedQuery = query?.Trim();

            var zoos = _zooRepository.GetAllZoos()
                .Where(zoo => string.IsNullOrWhiteSpace(normalizedQuery) ||
                              zoo.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(zoo => zoo.Name)
                .Select(zoo => new
                {
                    id = zoo.Id,
                    name = zoo.Name
                })
                .ToList();

            return Json(zoos);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult SearchZookeepers(string? query)
        {
            var normalizedQuery = query?.Trim();

            var zookeepers = _zooRepository.GetAllZookeepers()
                .Where(zookeeper => string.IsNullOrWhiteSpace(normalizedQuery) ||
                                    $"{zookeeper.FirstName} {zookeeper.LastName}".Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(zookeeper => zookeeper.FirstName)
                .ThenBy(zookeeper => zookeeper.LastName)
                .Select(zookeeper => new
                {
                    id = zookeeper.Id,
                    name = $"{zookeeper.FirstName} {zookeeper.LastName}"
                })
                .ToList();

            return Json(zookeepers);
        }

        public IActionResult Details(int id)
        {
            var enclosure = _zooRepository.GetEnclosureById(id);
            if (enclosure == null)
            {
                return NotFound();
            }

            var (statusText, statusClass) = GetOccupancyStatus(enclosure.Animals.Count, enclosure.Capacity);

            var viewModel = new EnclosureDetailsViewModel
            {
                Enclosure = enclosure,
                ZookeeperName = $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                ZooName = enclosure.Zoo.Name,
                OccupancyStatusText = statusText,
                OccupancyStatusClass = statusClass
            };

            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            var viewModel = new EnclosureCreateEditViewModel
            {
                Capacity = 5,
                AvailableZookeepers = _zooRepository.GetAllZookeepers()
                    .Select(z => new ZookeeperOption { Id = z.Id, Name = $"{z.FirstName} {z.LastName}" })
                    .ToList(),
                AvailableZoos = _zooRepository.GetAllZoos()
                    .Select(z => new ZooOption { Id = z.Id, Name = z.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(EnclosureCreateEditViewModel viewModel)
        {
            // Additional server-side validation
            ValidateEnclosureViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableZookeepers = _zooRepository.GetAllZookeepers()
                    .Select(z => new ZookeeperOption { Id = z.Id, Name = $"{z.FirstName} {z.LastName}" })
                    .ToList();
                viewModel.AvailableZoos = _zooRepository.GetAllZoos()
                    .Select(z => new ZooOption { Id = z.Id, Name = z.Name })
                    .ToList();
                return View(viewModel);
            }

            var enclosure = new Enclosure
            {
                Name = viewModel.Name,
                Type = viewModel.Type,
                Capacity = viewModel.Capacity,
                ZookeeperId = viewModel.ZookeeperId,
                ZooId = viewModel.ZooId
            };

            _zooRepository.AddEnclosure(enclosure);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = enclosure.Id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var enclosure = _zooRepository.GetEnclosureById(id.Value);
            if (enclosure == null)
                return NotFound();

            var viewModel = new EnclosureCreateEditViewModel
            {
                Id = enclosure.Id,
                Name = enclosure.Name,
                Type = enclosure.Type,
                Capacity = enclosure.Capacity,
                ZookeeperId = enclosure.ZookeeperId,
                ZooId = enclosure.ZooId,
                AvailableZookeepers = _zooRepository.GetAllZookeepers()
                    .Select(z => new ZookeeperOption { Id = z.Id, Name = $"{z.FirstName} {z.LastName}" })
                    .ToList(),
                AvailableZoos = _zooRepository.GetAllZoos()
                    .Select(z => new ZooOption { Id = z.Id, Name = z.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, EnclosureCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            // Additional server-side validation
            ValidateEnclosureViewModel(viewModel, id);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableZookeepers = _zooRepository.GetAllZookeepers()
                    .Select(z => new ZookeeperOption { Id = z.Id, Name = $"{z.FirstName} {z.LastName}" })
                    .ToList();
                viewModel.AvailableZoos = _zooRepository.GetAllZoos()
                    .Select(z => new ZooOption { Id = z.Id, Name = z.Name })
                    .ToList();
                return View(viewModel);
            }

            var enclosure = _zooRepository.GetEnclosureById(id);
            if (enclosure == null)
                return NotFound();

            enclosure.Name = viewModel.Name;
            enclosure.Type = viewModel.Type;
            enclosure.Capacity = viewModel.Capacity;
            enclosure.ZookeeperId = viewModel.ZookeeperId;
            enclosure.ZooId = viewModel.ZooId;

            _zooRepository.UpdateEnclosure(enclosure);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var enclosure = _zooRepository.GetEnclosureById(id.Value);
            if (enclosure == null)
                return NotFound();

            var (statusText, statusClass) = GetOccupancyStatus(enclosure.Animals.Count, enclosure.Capacity);

            var viewModel = new EnclosureDetailsViewModel
            {
                Enclosure = enclosure,
                ZookeeperName = $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                ZooName = enclosure.Zoo.Name,
                OccupancyStatusText = statusText,
                OccupancyStatusClass = statusClass
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var enclosure = _zooRepository.GetEnclosureById(id);
            if (enclosure == null)
                return NotFound();

            _zooRepository.DeleteEnclosure(id);
            _zooRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        private static (string statusText, string statusClass) GetOccupancyStatus(int animalsCount, int capacity)
        {
            if (animalsCount >= capacity)
            {
                return ("At capacity", "entity-chip--warning");
            }

            return ("Available space", "entity-chip--success");
        }

        private List<EnclosureListItemViewModel> GetEnclosureListItems(string? search)
        {
            var normalizedSearch = search?.Trim();

            return _zooRepository.GetAllEnclosures()
                .Select(enclosure =>
                {
                    var animalsCount = enclosure.Animals.Count;
                    var (statusText, statusClass) = GetOccupancyStatus(animalsCount, enclosure.Capacity);

                    return new EnclosureListItemViewModel
                    {
                        Id = enclosure.Id,
                        Name = enclosure.Name,
                        Type = enclosure.Type,
                        ZooName = enclosure.Zoo.Name,
                        Capacity = enclosure.Capacity,
                        AnimalsCount = animalsCount,
                        ZookeeperName = $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                        OccupancyStatusText = statusText,
                        OccupancyStatusClass = statusClass
                    };
                })
                .Where(item => string.IsNullOrWhiteSpace(normalizedSearch) ||
                               item.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.Name)
                .ToList();
        }

        private void ValidateEnclosureViewModel(EnclosureCreateEditViewModel vm, int? existingEnclosureId = null)
        {
            // Verify zookeeper exists
            var zookeeperExists = _zooRepository.GetAllZookeepers().Any(z => z.Id == vm.ZookeeperId);
            if (!zookeeperExists)
            {
                ModelState.AddModelError(nameof(vm.ZookeeperId), "Selected zookeeper does not exist.");
            }

            // Verify zoo exists
            var zooExists = _zooRepository.GetAllZoos().Any(z => z.Id == vm.ZooId);
            if (!zooExists)
            {
                ModelState.AddModelError(nameof(vm.ZooId), "Selected zoo does not exist.");
            }

            // If editing, ensure capacity isn't below current animals count
            if (existingEnclosureId.HasValue)
            {
                var enclosure = _zooRepository.GetEnclosureById(existingEnclosureId.Value);
                if (enclosure != null)
                {
                    var animalsCount = enclosure.Animals.Count;
                    if (vm.Capacity < animalsCount)
                    {
                        ModelState.AddModelError(nameof(vm.Capacity), $"Capacity cannot be less than current animals count ({animalsCount}).");
                    }
                }
            }
        }
    }
}
