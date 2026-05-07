using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class Zoo
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Location { get; set; }
        
        // Navigation properties
        public virtual ICollection<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
    }
}