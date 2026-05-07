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
            var enclosures = _zooRepository.GetAllEnclosures().ToList();

            var feedings = _zooRepository.GetAllFeedings()
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
                .OrderBy(f => f.FeedingTime)
                .ToList();

            return View(feedings);
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

        private static (string statusText, string statusClass) GetFeedingStatus(DateTime feedingTime)
        {
            if (feedingTime >= DateTime.Now)
            {
                return ("Scheduled", "entity-chip--success");
            }

            return ("Completed", "entity-chip--role");
        }
    }
}
