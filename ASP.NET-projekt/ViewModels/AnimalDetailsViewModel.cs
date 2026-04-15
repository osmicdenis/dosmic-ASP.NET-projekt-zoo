using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class AnimalDetailsViewModel
    {
        public required Animal Animal { get; set; }
        public required string EnclosureName { get; set; }
        public required string EnclosureType { get; set; }
        public required string ZookeeperName { get; set; }
        public required IEnumerable<Animal> EnclosureMates { get; set; }
    }
}
