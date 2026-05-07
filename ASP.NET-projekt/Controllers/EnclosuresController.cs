using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
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
            var enclosures = _zooRepository.GetAllEnclosures()
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
                .OrderBy(e => e.Name)
                .ToList();

            return View(enclosures);
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

        private static (string statusText, string statusClass) GetOccupancyStatus(int animalsCount, int capacity)
        {
            if (animalsCount >= capacity)
            {
                return ("At capacity", "entity-chip--warning");
            }

            return ("Available space", "entity-chip--success");
        }
    }
}
