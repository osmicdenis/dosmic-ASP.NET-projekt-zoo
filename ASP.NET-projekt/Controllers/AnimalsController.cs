using System;
using System.Linq;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ASP.NET_projekt.Models;
using Microsoft.AspNetCore.Hosting;
using AnimalModel = ASP.NET_projekt.Models.Animal;

namespace ASP.NET_projekt.Controllers
{
    public class AnimalsController : Controller
    {
        private readonly IZooRepository _zooRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AnimalsController(IZooRepository zooRepository, IWebHostEnvironment webHostEnvironment)
        {
            _zooRepository = zooRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string? search)
        {
            Console.WriteLine($"Animals MVC Index called. search={search}");

            ViewBag.SearchQuery = search ?? string.Empty;
            return View(GetAnimalListItems(search));
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            Console.WriteLine($"Animals MVC Search called. query={query}");

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
            Console.WriteLine($"Animals MVC Details called. id={id}");

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
                    : $"{enclosure.Zookeeper!.FirstName} {enclosure.Zookeeper.LastName}",
                EnclosureMates = enclosure == null
                    ? Enumerable.Empty<AnimalModel>()
                    : enclosure.Animals.Where(a => a.Id != animal.Id).OrderBy(a => a.Name)
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Photos(int animalId)
        {
            Console.WriteLine($"Animals MVC Photos called. animalId={animalId}");

            var animal = _zooRepository.GetAnimalById(animalId);
            if (animal == null)
            {
                return NotFound();
            }

            return PartialView("_AnimalPhotos", animal.Photos.OrderByDescending(photo => photo.CreatedAt));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult UploadPhoto(int animalId, IFormFile file)
        {
            Console.WriteLine($"Animals MVC UploadPhoto called. animalId={animalId}, file={(file == null ? "<null>" : file.FileName)}");

            var animal = _zooRepository.GetAnimalById(animalId);
            if (animal == null)
            {
                return NotFound();
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Please choose an image file.");
            }

            var uploadsRoot = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "animals", animalId.ToString());
            Directory.CreateDirectory(uploadsRoot);

            var fileExtension = Path.GetExtension(file.FileName);
            var storedFileName = $"{Guid.NewGuid():N}{fileExtension}";
            var physicalPath = Path.Combine(uploadsRoot, storedFileName);
            var relativePath = $"/uploads/animals/{animalId}/{storedFileName}";

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var animalPhoto = new AnimalPhoto
            {
                AnimalId = animalId,
                FileName = file.FileName,
                FilePath = relativePath,
                ContentType = file.ContentType,
                FileSize = file.Length,
                CreatedAt = DateTime.UtcNow
            };

            _zooRepository.AddAnimalPhoto(animalPhoto);
            _zooRepository.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult DeletePhoto(int id)
        {
            Console.WriteLine($"Animals MVC DeletePhoto called. id={id}");

            var animalPhoto = _zooRepository.GetAnimalPhotoById(id);
            if (animalPhoto == null)
            {
                return NotFound();
            }

            var physicalPath = Path.Combine(_webHostEnvironment.WebRootPath, animalPhoto.FilePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _zooRepository.DeleteAnimalPhoto(id);
            _zooRepository.SaveChanges();

            return Json(new { success = true });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            Console.WriteLine("Animals MVC Create GET called.");

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
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AnimalCreateEditViewModel viewModel)
        {
            Console.WriteLine($"Animals MVC Create POST called. name={viewModel.Name}, species={viewModel.Species}");

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
                Name = viewModel.Name!,
                Species = viewModel.Species!,
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
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Edit(int? id)
        {
            Console.WriteLine($"Animals MVC Edit GET called. id={id}");

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
        [Authorize(Roles = "Admin,Manager")]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, AnimalCreateEditViewModel viewModel)
        {
            Console.WriteLine($"Animals MVC Edit POST called. id={id}");

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

            animal.Name = viewModel.Name!;
            animal.Species = viewModel.Species!;
            animal.DateOfBirth = viewModel.DateOfBirth;
            animal.DateOfArrival = viewModel.DateOfArrival;
            animal.Diet = viewModel.Diet;
            animal.EnclosureId = viewModel.EnclosureId;

            _zooRepository.UpdateAnimal(animal);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int? id)
        {
            Console.WriteLine($"Animals MVC Delete GET called. id={id}");

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
                    : $"{enclosure.Zookeeper!.FirstName} {enclosure.Zookeeper.LastName}",
                EnclosureMates = enclosure == null
                    ? Enumerable.Empty<AnimalModel>()
                    : enclosure.Animals.Where(a => a.Id != animal.Id).OrderBy(a => a.Name)
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            Console.WriteLine($"Animals MVC Delete POST called. id={id}");

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
                            : $"{enclosure.Zookeeper!.FirstName} {enclosure.Zookeeper.LastName}",
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
