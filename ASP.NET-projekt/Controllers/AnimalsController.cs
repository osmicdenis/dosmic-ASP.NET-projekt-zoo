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
    }
}
