using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class Food
    {
        [Key]
        public int Id { get; set; }
        public required string Name { get; set; }
        
        // Navigation properties
        public virtual ICollection<Feeding> Feedings { get; set; } = new List<Feeding>();
    }
}