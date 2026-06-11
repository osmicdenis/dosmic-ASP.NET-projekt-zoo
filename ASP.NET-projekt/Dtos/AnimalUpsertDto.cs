using ASP.NET_projekt.Models;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class AnimalUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Species { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public DateTime DateOfArrival { get; set; }

        [Required]
        public DietType Diet { get; set; }

        public int? EnclosureId { get; set; }
    }
}