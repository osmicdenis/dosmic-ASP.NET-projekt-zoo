using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class ZooDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Location { get; set; }

        public List<LookupDto> Enclosures { get; set; } = new();

        public int EnclosuresCount => Enclosures.Count;
    }

    public class ZooUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? Location { get; set; }
    }
}