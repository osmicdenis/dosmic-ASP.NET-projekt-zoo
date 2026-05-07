using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Index()
        {
            var enclosures = _zooRepository.GetAllEnclosures().ToList();

            var animals = _zooRepository.GetAllAnimals()
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
                .OrderBy(a => a.Name)
                .ToList();

            return View(animals);
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
    }
}
