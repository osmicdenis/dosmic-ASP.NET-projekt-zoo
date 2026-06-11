using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class FeedingDto
    {
        public int Id { get; set; }

        public required AnimalSummaryDto Animal { get; set; }

        public required FoodSummaryDto Food { get; set; }

        public EnclosureSummaryDto? Enclosure { get; set; }

        public DateTime FeedingTime { get; set; }
    }

    public class FeedingUpsertDto
    {
        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int FoodId { get; set; }

        [Required]
        public DateTime FeedingTime { get; set; }
    }
}