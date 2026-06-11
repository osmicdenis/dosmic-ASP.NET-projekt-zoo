using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.ViewModels
{
    public class FeedingCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Animal is required")]
        public int AnimalId { get; set; }

        [Required(ErrorMessage = "Food is required")]
        public int FoodId { get; set; }

        [Required(ErrorMessage = "Feeding time is required")]
        [DataType(DataType.DateTime)]
        public DateTime FeedingTime { get; set; }

        public List<AnimalOption> AvailableAnimals { get; set; } = new List<AnimalOption>();
        public List<FoodOption> AvailableFoods { get; set; } = new List<FoodOption>();
    }

    public class AnimalOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class FoodOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
