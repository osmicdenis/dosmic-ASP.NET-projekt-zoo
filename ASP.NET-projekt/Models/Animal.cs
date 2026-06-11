using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class Animal
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfArrival { get; set; }
        public DietType Diet { get; set; }
        
        // Foreign key to Enclosure
        public int? EnclosureId { get; set; }
        public virtual Enclosure? Enclosure { get; set; }
        
        // Navigation properties
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public virtual ICollection<Feeding> Feedings { get; set; } = new List<Feeding>();
        public virtual ICollection<AnimalPhoto> Photos { get; set; } = new List<AnimalPhoto>();
    }
}
