using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_projekt.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }
        public required string Diagnosis { get; set; }
        public required string Therapy { get; set; }
        public DateTime ExaminationDate { get; set; }
        
        // Foreign key to Veterinarian
        [ForeignKey("Veterinarian")]
        public int VeterinarianId { get; set; }
        public virtual Veterinarian? Veterinarian { get; set; }
        
        // Foreign key to Animal
        [ForeignKey("Animal")]
        public int AnimalId { get; set; }
        public virtual Animal? Animal { get; set; }
    }
}