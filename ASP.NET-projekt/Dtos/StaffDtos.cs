using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class ZookeeperDto
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public int YearsOfExperience { get; set; }

        public DateTime DateOfEmployment { get; set; }

        public int EnclosuresCount { get; set; }

        public int AnimalsCount { get; set; }
    }

    public class ZookeeperUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; }

        [Range(0, 100)]
        public int YearsOfExperience { get; set; }

        public DateTime DateOfEmployment { get; set; }
    }

    public class VeterinarianDto
    {
        public int Id { get; set; }

        public required string FirstName { get; set; }

        public required string LastName { get; set; }

        public int MedicalRecordsCount { get; set; }

        public int TreatedAnimalsCount { get; set; }
    }

    public class VeterinarianUpsertDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; }
    }
}