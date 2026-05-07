using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_projekt.Models
{
    public class Enclosure
    {
        [Key]
        public int Id { get; set; }
        
        // Foreign key to Zoo
        [ForeignKey("Zoo")]
        public int ZooId { get; set; }
        public virtual Zoo? Zoo { get; set; }
        
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Capacity { get; set; }

        // Foreign key to Zookeeper
        [ForeignKey("Zookeeper")]
        public int ZookeeperId { get; set; }
        public virtual Zookeeper? Zookeeper { get; set; }
        
        // Navigation properties
        public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
    }
}