using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class FeedingListItemViewModel
    {
        public int Id { get; set; }
        public required string AnimalName { get; set; }
        public required string AnimalSpecies { get; set; }
        public required DietType Diet { get; set; }
        public required string FoodName { get; set; }
        public DateTime FeedingTime { get; set; }
        public required string EnclosureName { get; set; }
        public required string ZookeeperName { get; set; }
        public required string StatusText { get; set; }
        public required string StatusClass { get; set; }
    }
}
