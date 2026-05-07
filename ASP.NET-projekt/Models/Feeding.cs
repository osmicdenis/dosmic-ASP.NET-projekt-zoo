using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_projekt.Models
{
    public class Feeding
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Animal")]
        public int AnimalId { get; set; }
        public virtual Animal? Animal { get; set; }

        [ForeignKey("Food")]
        public int FoodId { get; set; }
        public virtual Food? Food { get; set; }
        
        public DateTime FeedingTime { get; set; }
    }
}