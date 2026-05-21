using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class EnclosureDetailsViewModel
    {
        public required Enclosure Enclosure { get; set; }
        public required string ZookeeperName { get; set; }
        public required string ZooName { get; set; }
        public required string OccupancyStatusText { get; set; }
        public required string OccupancyStatusClass { get; set; }
        public int Capacity => Enclosure.Capacity;
        public int AnimalsCount => Enclosure.Animals.Count;
        public string OccupancyDisplay => $"{AnimalsCount}/{Capacity}";
    }
}
