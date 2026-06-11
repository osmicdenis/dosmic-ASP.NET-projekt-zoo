using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class EnclosureDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Type { get; set; }

        public int Capacity { get; set; }

        public required ZooSummaryDto Zoo { get; set; }

        public required ZookeeperSummaryDto Zookeeper { get; set; }

        public List<AnimalSummaryDto> Animals { get; set; } = new();

        public int AnimalsCount => Animals.Count;
    }

    public class EnclosureUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Type { get; set; }

        [Range(1, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        public int ZooId { get; set; }

        [Required]
        public int ZookeeperId { get; set; }
    }
}