using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Dtos
{
    public class MedicalRecordDto
    {
        public int Id { get; set; }

        public required string Diagnosis { get; set; }

        public required string Therapy { get; set; }

        public DateTime ExaminationDate { get; set; }

        public required AnimalSummaryDto Animal { get; set; }

        public required VeterinarianSummaryDto Veterinarian { get; set; }

        public EnclosureSummaryDto? Enclosure { get; set; }
    }

    public class MedicalRecordUpsertDto
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? Diagnosis { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string? Therapy { get; set; }

        [Required]
        public DateTime ExaminationDate { get; set; }

        [Required]
        public int AnimalId { get; set; }

        [Required]
        public int VeterinarianId { get; set; }
    }
}