using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers
{
    public class FeedingsController : Controller
    {
        private readonly IZooRepository _zooRepository;

        public FeedingsController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        public IActionResult Index()
        {
            return View(GetFeedingListItems(null));
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            var feedings = GetFeedingListItems(query)
                .Select(feeding => new
                {
                    id = feeding.Id,
                    animalName = feeding.AnimalName,
                    animalSpecies = feeding.AnimalSpecies,
                    diet = feeding.Diet.ToString(),
                    foodName = feeding.FoodName,
                    feedingTime = feeding.FeedingTime,
                    enclosureName = feeding.EnclosureName,
                    zookeeperName = feeding.ZookeeperName,
                    statusText = feeding.StatusText,
                    statusClass = feeding.StatusClass
                })
                .ToList();

            return Json(feedings);
        }

        [HttpGet]
        public IActionResult SearchAnimals(string? query)
        {
            var normalizedQuery = query?.Trim();

            var animals = _zooRepository.GetAllAnimals()
                .Where(animal => string.IsNullOrWhiteSpace(normalizedQuery) ||
                                 animal.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                                 animal.Species.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(animal => animal.Name)
                .Select(animal => new
                {
                    id = animal.Id,
                    name = $"{animal.Name} ({animal.Species})"
                })
                .ToList();

            return Json(animals);
        }

        [HttpGet]
        public IActionResult SearchFoods(string? query)
        {
            var normalizedQuery = query?.Trim();

            var foods = _zooRepository.GetAllFoods()
                .Where(food => string.IsNullOrWhiteSpace(normalizedQuery) ||
                               food.Name.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase))
                .OrderBy(food => food.Name)
                .Select(food => new
                {
                    id = food.Id,
                    name = food.Name
                })
                .ToList();

            return Json(foods);
        }

        public IActionResult Details(int id)
        {
            var feeding = _zooRepository.GetFeedingById(id);
            if (feeding == null)
            {
                return NotFound();
            }

            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(e => e.Animals.Any(a => a.Id == feeding.AnimalId));
            var (statusText, statusClass) = GetFeedingStatus(feeding.FeedingTime);

            var model = new FeedingDetailsViewModel
            {
                Feeding = feeding,
                EnclosureName = enclosure?.Name ?? "Not Assigned",
                EnclosureType = enclosure?.Type ?? "N/A",
                ZookeeperName = enclosure == null
                    ? "Not Assigned"
                    : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                ZooName = enclosure?.Zoo.Name ?? "Unknown Zoo",
                StatusText = statusText,
                StatusClass = statusClass,
                OtherFeedingsForAnimal = _zooRepository.GetFeedingsByAnimalId(feeding.AnimalId)
                    .Where(f => f.Id != feeding.Id)
                    .OrderByDescending(f => f.FeedingTime)
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new FeedingCreateEditViewModel
            {
                FeedingTime = DateTime.Now,
                AvailableAnimals = _zooRepository.GetAllAnimals()
                    .Select(a => new AnimalOption { Id = a.Id, Name = a.Name })
                    .ToList(),
                AvailableFoods = _zooRepository.GetAllFoods()
                    .Select(f => new FoodOption { Id = f.Id, Name = f.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(FeedingCreateEditViewModel viewModel)
        {
            // Additional server-side validation
            ValidateFeedingViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableAnimals = _zooRepository.GetAllAnimals()
                    .Select(a => new AnimalOption { Id = a.Id, Name = a.Name })
                    .ToList();
                viewModel.AvailableFoods = _zooRepository.GetAllFoods()
                    .Select(f => new FoodOption { Id = f.Id, Name = f.Name })
                    .ToList();
                return View(viewModel);
            }

            var feeding = new Feeding
            {
                AnimalId = viewModel.AnimalId,
                FoodId = viewModel.FoodId,
                FeedingTime = viewModel.FeedingTime
            };

            _zooRepository.AddFeeding(feeding);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = feeding.Id });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var feeding = _zooRepository.GetFeedingById(id.Value);
            if (feeding == null)
                return NotFound();

            var viewModel = new FeedingCreateEditViewModel
            {
                Id = feeding.Id,
                AnimalId = feeding.AnimalId,
                FoodId = feeding.FoodId,
                FeedingTime = feeding.FeedingTime,
                AvailableAnimals = _zooRepository.GetAllAnimals()
                    .Select(a => new AnimalOption { Id = a.Id, Name = a.Name })
                    .ToList(),
                AvailableFoods = _zooRepository.GetAllFoods()
                    .Select(f => new FoodOption { Id = f.Id, Name = f.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, FeedingCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            // Additional server-side validation
            ValidateFeedingViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableAnimals = _zooRepository.GetAllAnimals()
                    .Select(a => new AnimalOption { Id = a.Id, Name = a.Name })
                    .ToList();
                viewModel.AvailableFoods = _zooRepository.GetAllFoods()
                    .Select(f => new FoodOption { Id = f.Id, Name = f.Name })
                    .ToList();
                return View(viewModel);
            }

            var feeding = _zooRepository.GetFeedingById(id);
            if (feeding == null)
                return NotFound();

            feeding.AnimalId = viewModel.AnimalId;
            feeding.FoodId = viewModel.FoodId;
            feeding.FeedingTime = viewModel.FeedingTime;

            _zooRepository.UpdateFeeding(feeding);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var feeding = _zooRepository.GetFeedingById(id.Value);
            if (feeding == null)
                return NotFound();

            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(e => e.Animals.Any(a => a.Id == feeding.AnimalId));
            var (statusText, statusClass) = GetFeedingStatus(feeding.FeedingTime);

            var model = new FeedingDetailsViewModel
            {
                Feeding = feeding,
                EnclosureName = enclosure?.Name ?? "Not Assigned",
                EnclosureType = enclosure?.Type ?? "N/A",
                ZookeeperName = enclosure == null
                    ? "Not Assigned"
                    : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                ZooName = enclosure?.Zoo.Name ?? "Unknown Zoo",
                StatusText = statusText,
                StatusClass = statusClass,
                OtherFeedingsForAnimal = _zooRepository.GetFeedingsByAnimalId(feeding.AnimalId)
                    .Where(f => f.Id != feeding.Id)
                    .OrderByDescending(f => f.FeedingTime)
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var feeding = _zooRepository.GetFeedingById(id);
            if (feeding == null)
                return NotFound();

            _zooRepository.DeleteFeeding(id);
            _zooRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        private static (string statusText, string statusClass) GetFeedingStatus(DateTime feedingTime)
        {
            if (feedingTime >= DateTime.Now)
            {
                return ("Scheduled", "entity-chip--success");
            }

            return ("Completed", "entity-chip--role");
        }

        private List<FeedingListItemViewModel> GetFeedingListItems(string? search)
        {
            var normalizedSearch = search?.Trim();
            var enclosures = _zooRepository.GetAllEnclosures().ToList();

            return _zooRepository.GetAllFeedings()
                .Select(feeding =>
                {
                    var enclosure = enclosures.FirstOrDefault(e => e.Animals.Any(a => a.Id == feeding.AnimalId));
                    var (statusText, statusClass) = GetFeedingStatus(feeding.FeedingTime);

                    return new FeedingListItemViewModel
                    {
                        Id = feeding.Id,
                        AnimalName = feeding.Animal.Name,
                        AnimalSpecies = feeding.Animal.Species,
                        Diet = feeding.Animal.Diet,
                        FoodName = feeding.Food.Name,
                        FeedingTime = feeding.FeedingTime,
                        EnclosureName = enclosure?.Name ?? "Not Assigned",
                        ZookeeperName = enclosure == null
                            ? "Not Assigned"
                            : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                        StatusText = statusText,
                        StatusClass = statusClass
                    };
                })
                .Where(item => string.IsNullOrWhiteSpace(normalizedSearch) ||
                               item.AnimalName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.AnimalSpecies.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.FoodName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.EnclosureName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.ZookeeperName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.FeedingTime)
                .ToList();
        }

        private void ValidateFeedingViewModel(FeedingCreateEditViewModel vm)
        {
            // Ensure referenced animal exists
            var animal = _zooRepository.GetAnimalById(vm.AnimalId);
            if (animal == null)
            {
                ModelState.AddModelError(nameof(vm.AnimalId), "Selected animal does not exist.");
            }

            // Ensure referenced food exists
            var foodExists = _zooRepository.GetAllFoods().Any(f => f.Id == vm.FoodId);
            if (!foodExists)
            {
                ModelState.AddModelError(nameof(vm.FoodId), "Selected food does not exist.");
            }

            // If animal exists, feeding time should not be before animal arrival
            if (animal != null)
            {
                if (vm.FeedingTime < animal.DateOfArrival)
                {
                    ModelState.AddModelError(nameof(vm.FeedingTime), "Feeding time cannot be before the animal's arrival date.");
                }
            }
        }
    }
}
