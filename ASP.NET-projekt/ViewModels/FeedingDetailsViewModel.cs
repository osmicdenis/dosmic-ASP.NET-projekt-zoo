using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class FeedingDetailsViewModel
    {
        public required Feeding Feeding { get; set; }
        public required string EnclosureName { get; set; }
        public required string EnclosureType { get; set; }
        public required string ZookeeperName { get; set; }
        public required string ZooName { get; set; }
        public required string StatusText { get; set; }
        public required string StatusClass { get; set; }
        public required IEnumerable<Feeding> OtherFeedingsForAnimal { get; set; }
    }
}
