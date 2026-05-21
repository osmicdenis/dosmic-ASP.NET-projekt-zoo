using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class Zookeeper
    {
        [Key]
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int YearsOfExperience { get; set; }
        public DateTime DateOfEmployment { get; set; }

        // Navigation properties
        public virtual ICollection<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
    }
}