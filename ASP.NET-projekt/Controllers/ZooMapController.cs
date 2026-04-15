using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers
{
    public class ZooMapController : Controller
    {
        private readonly IZooRepository _zooRepository;

        public ZooMapController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        public IActionResult Index()
        {
            var hotspotPositions = new Dictionary<string, (double Left, double Top)>
            {
                ["Lion Rock"] = (66.5, 36.0),
                ["Gorilla Grove"] = (33.0, 34.0),
                ["Reptile House"] = (44.0, 44.5),
                ["Elephant Plains"] = (72.5, 27.0),
                ["Giraffe Terrace"] = (52.0, 33.0),
                ["Parrot Aviary"] = (20.5, 41.5)
            };

            var hotspots = _zooRepository.GetAllEnclosures()
                .Where(enclosure => hotspotPositions.ContainsKey(enclosure.Name))
                .Select(enclosure =>
                {
                    var position = hotspotPositions[enclosure.Name];
                    var animalCount = enclosure.Animals.Count;
                    var occupancyText = $"{animalCount}/{enclosure.Capacity}";
                    var occupancyStatus = animalCount >= enclosure.Capacity
                        ? ("At capacity", "entity-chip--warning")
                        : ("Available space", "entity-chip--success");

                    return new ZooMapHotspotViewModel
                    {
                        EnclosureId = enclosure.Id,
                        EnclosureName = enclosure.Name,
                        EnclosureType = enclosure.Type,
                        ZookeeperName = $"{enclosure.Zookeeper.FirstName} {enclosure.Zookeeper.LastName}",
                        Capacity = enclosure.Capacity,
                        AnimalCount = animalCount,
                        OccupancyDisplay = occupancyText,
                        OccupancyStatusText = occupancyStatus.Item1,
                        OccupancyStatusClass = occupancyStatus.Item2,
                        LeftPercent = position.Left,
                        TopPercent = position.Top,
                        AnimalSummaries = enclosure.Animals
                            .OrderBy(animal => animal.Name)
                            .Select(animal => $"{animal.Name} ({animal.Species})")
                            .ToList()
                    };
                })
                .OrderBy(hotspot => hotspot.EnclosureName)
                .ToList();

            var model = new ZooMapViewModel
            {
                MapImagePath = Url.Action(nameof(Image), "ZooMap") ?? "/ZooMap/Image",
                Hotspots = hotspots
            };

            return View(model);
        }

        public IActionResult Image()
        {
            var mapPath = Path.Combine(Directory.GetCurrentDirectory(), "Views", "zoomap.png");
            if (!System.IO.File.Exists(mapPath))
            {
                return NotFound();
            }

            return PhysicalFile(mapPath, "image/png");
        }
    }
}
