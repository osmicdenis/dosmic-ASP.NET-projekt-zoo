using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class FoodDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public int FeedingsCount { get; set; }
    }

    public class FoodUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }
    }
}