using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class Veterinarian
    {
        [Key]
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        
        // Navigation properties
        public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}