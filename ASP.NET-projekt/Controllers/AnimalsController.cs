using System;
using System.Linq;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_projekt.Models;
using AnimalModel = ASP.NET_projekt.Models.Animal;

namespace ASP.NET_projekt.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly IZooRepository _zooRepository;

        public AnimalsController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        public IActionResult Index(string? search)
        {
            ViewBag.SearchQuery = search ?? string.Empty;
            return View(GetAnimalListItems(search));
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            var animals = GetAnimalListItems(query)
                .Select(animal => new
                {
                    id = animal.Id,
                    name = animal.Name,
                    species = animal.Species,
                    diet = animal.Diet.ToString(),
                    age = animal.Age,
                    enclosureName = animal.EnclosureName,
                    medicalRecordsCount = animal.MedicalRecordsCount
                })
                .ToList();

            return Json(animals);
        }

        public IActionResult Details(int id)
        {
            var animal = _zooRepository.GetAnimalById(id);
            if (animal == null)
            {
                return NotFound();
            }

            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(e => e.Animals.Any(a => a.Id == id));

            var viewModel = new AnimalDetailsViewModel
            {
                Animal = animal,
                EnclosureName = enclosure?.Name ?? "Not Assigned",
                EnclosureType = enclosure?.Type ?? "N/A",
                ZookeeperName = enclosure == null
                    ? "Not Assigned"
                    : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                EnclosureMates = enclosure == null
                    ? Enumerable.Empty<AnimalModel>()
                    : enclosure.Animals.Where(a => a.Id != animal.Id).OrderBy(a => a.Name)
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new AnimalCreateEditViewModel
            {
                DateOfArrival = DateTime.Now,
                DateOfBirth = DateTime.Now.AddYears(-5),
                AvailableEnclosures = _zooRepository.GetAllEnclosures()
                    .Select(e => new EnclosureOption { Id = e.Id, Name = e.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AnimalCreateEditViewModel viewModel)
        {
            // Additional server-side validation beyond DataAnnotations
            ValidateAnimalViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableEnclosures = _zooRepository.GetAllEnclosures()
                    .Select(e => new EnclosureOption { Id = e.Id, Name = e.Name })
                    .ToList();
                return View(viewModel);
            }

            var animal = new AnimalModel
            {
                Name = viewModel.Name,
                Species = viewModel.Species,
                DateOfBirth = viewModel.DateOfBirth,
                DateOfArrival = viewModel.DateOfArrival,
                Diet = viewModel.Diet,
                EnclosureId = viewModel.EnclosureId
            };

            _zooRepository.AddAnimal(animal);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = animal.Id });
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var animal = _zooRepository.GetAnimalById(id.Value);
            if (animal == null)
                return NotFound();

            var viewModel = new AnimalCreateEditViewModel
            {
                Id = animal.Id,
                Name = animal.Name,
                Species = animal.Species,
                DateOfBirth = animal.DateOfBirth,
                DateOfArrival = animal.DateOfArrival,
                Diet = animal.Diet,
                EnclosureId = animal.EnclosureId,
                AvailableEnclosures = _zooRepository.GetAllEnclosures()
                    .Select(e => new EnclosureOption { Id = e.Id, Name = e.Name })
                    .ToList()
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AnimalCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            // Additional server-side validation beyond DataAnnotations
            ValidateAnimalViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                viewModel.AvailableEnclosures = _zooRepository.GetAllEnclosures()
                    .Select(e => new EnclosureOption { Id = e.Id, Name = e.Name })
                    .ToList();
                return View(viewModel);
            }

            var animal = _zooRepository.GetAnimalById(id);
            if (animal == null)
                return NotFound();

            animal.Name = viewModel.Name;
            animal.Species = viewModel.Species;
            animal.DateOfBirth = viewModel.DateOfBirth;
            animal.DateOfArrival = viewModel.DateOfArrival;
            animal.Diet = viewModel.Diet;
            animal.EnclosureId = viewModel.EnclosureId;

            _zooRepository.UpdateAnimal(animal);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var animal = _zooRepository.GetAnimalById(id.Value);
            if (animal == null)
                return NotFound();

            var enclosure = _zooRepository.GetAllEnclosures().FirstOrDefault(e => e.Animals.Any(a => a.Id == id));

            var viewModel = new AnimalDetailsViewModel
            {
                Animal = animal,
                EnclosureName = enclosure?.Name ?? "Not Assigned",
                EnclosureType = enclosure?.Type ?? "N/A",
                ZookeeperName = enclosure == null
                    ? "Not Assigned"
                    : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                EnclosureMates = enclosure == null
                    ? Enumerable.Empty<AnimalModel>()
                    : enclosure.Animals.Where(a => a.Id != animal.Id).OrderBy(a => a.Name)
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var animal = _zooRepository.GetAnimalById(id);
            if (animal == null)
                return NotFound();

            _zooRepository.DeleteAnimal(id);
            _zooRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        private List<AnimalListItemViewModel> GetAnimalListItems(string? search)
        {
            var normalizedSearch = search?.Trim();
            var enclosures = _zooRepository.GetAllEnclosures().ToList();

            return _zooRepository.GetAllAnimals()
                .Where(animal => string.IsNullOrWhiteSpace(normalizedSearch) ||
                                 animal.Name.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                                 animal.Species.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .Select(animal =>
                {
                    var enclosure = enclosures.FirstOrDefault(e => e.Animals.Any(a => a.Id == animal.Id));

                    return new AnimalListItemViewModel
                    {
                        Id = animal.Id,
                        Name = animal.Name,
                        Species = animal.Species,
                        Diet = animal.Diet,
                        DateOfBirth = animal.DateOfBirth,
                        EnclosureName = enclosure?.Name ?? "Not Assigned",
                        ZookeeperName = enclosure == null
                            ? "Not Assigned"
                            : $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                        MedicalRecordsCount = animal.MedicalRecords.Count
                    };
                })
                .OrderBy(animal => animal.Name)
                .ToList();
        }

        private void ValidateAnimalViewModel(AnimalCreateEditViewModel vm)
        {
            // Only perform these checks if model binding produced values (avoid duplicate errors when binding failed)
            // Date checks
            try
            {
                var now = DateTime.Now;
                if (vm.DateOfBirth > now)
                {
                    ModelState.AddModelError(nameof(vm.DateOfBirth), "Date of birth cannot be in the future.");
                }

                if (vm.DateOfArrival > now)
                {
                    ModelState.AddModelError(nameof(vm.DateOfArrival), "Date of arrival cannot be in the future.");
                }

                if (vm.DateOfBirth > vm.DateOfArrival)
                {
                    ModelState.AddModelError(nameof(vm.DateOfBirth), "Date of birth must be before or equal to date of arrival.");
                }
            }
            catch
            {
                // If any DateTime values weren't bound correctly, let ModelState hold the binding errors.
            }

            // Enclosure existence check
            if (vm.EnclosureId.HasValue)
            {
                var exists = _zooRepository.GetAllEnclosures().Any(e => e.Id == vm.EnclosureId.Value);
                if (!exists)
                {
                    ModelState.AddModelError(nameof(vm.EnclosureId), "Selected enclosure does not exist.");
                }
            }

            // Enum validation for Diet
            if (!Enum.IsDefined(typeof(Models.DietType), vm.Diet))
            {
                ModelState.AddModelError(nameof(vm.Diet), "Invalid diet type selected.");
            }
        }
    }
}
