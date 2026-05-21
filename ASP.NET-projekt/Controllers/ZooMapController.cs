using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers
{
    public class ZooMapController : Controller
    {
        private readonly IZooRepository _zooRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ZooMapController(IZooRepository zooRepository, IWebHostEnvironment webHostEnvironment)
        {
            _zooRepository = zooRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var hotspotPositions = new Dictionary<string, (double Left, double Top)>
            {
                ["African Savanna"] = (66.5, 36.0),
                ["Jungle Canopy"] = (33.0, 34.0),
                ["Arctic Zone"] = (44.0, 44.5),
                ["Marine Aquarium"] = (72.5, 27.0)
            };

            var hotspots = _zooRepository.GetAllEnclosures()
                .Select(enclosure =>
                {
                    var currentEnclosure = enclosure ?? throw new InvalidOperationException("Enclosure cannot be null.");
                    var enclosureName = currentEnclosure.Name ?? "Unknown";
                    var enclosureType = currentEnclosure.Type ?? "Unknown";
                    hotspotPositions.TryGetValue(enclosureName, out var position);
                    var animalCount = currentEnclosure.Animals.Count;
                    var zookeeper = currentEnclosure.Zookeeper;
                    var occupancyText = $"{animalCount}/{currentEnclosure.Capacity}";
                    var occupancyStatus = animalCount >= currentEnclosure.Capacity
                        ? ("At capacity", "entity-chip--warning")
                        : ("Available space", "entity-chip--success");

                    return new ZooMapHotspotViewModel
                    {
                        EnclosureId = currentEnclosure.Id,
                        EnclosureName = enclosureName,
                        EnclosureType = enclosureType,
                        ZookeeperName = zookeeper == null
                            ? "Not Assigned"
                            : $"{zookeeper.FirstName} {zookeeper.LastName}",
                        Capacity = currentEnclosure.Capacity,
                        AnimalCount = animalCount,
                        OccupancyDisplay = occupancyText,
                        OccupancyStatusText = occupancyStatus.Item1,
                        OccupancyStatusClass = occupancyStatus.Item2,
                        LeftPercent = position.Left,
                        TopPercent = position.Top,
                        AnimalSummaries = currentEnclosure.Animals
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
            var mapPath = Path.Combine(_webHostEnvironment.ContentRootPath, "Views", "zoomap.png");
            if (!System.IO.File.Exists(mapPath))
            {
                return NotFound();
            }

            return PhysicalFile(mapPath, "image/png");
        }
    }
}
