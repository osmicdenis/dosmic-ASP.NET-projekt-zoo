using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.ViewModels
{
    public class EnclosureCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Type must be between 3 and 100 characters")]
        public string Type { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required")]
        [Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Zookeeper is required")]
        public int ZookeeperId { get; set; }

        [Required(ErrorMessage = "Zoo is required")]
        public int ZooId { get; set; }

        public List<ZookeeperOption> AvailableZookeepers { get; set; } = new List<ZookeeperOption>();
        public List<ZooOption> AvailableZoos { get; set; } = new List<ZooOption>();
    }

    public class ZookeeperOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ZooOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
