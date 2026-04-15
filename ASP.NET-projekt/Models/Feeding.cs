namespace ASP.NET_projekt.Models
{
    public class Feeding
    {
        public int Id { get; set; }

        public int AnimalId { get; set; }
        public required Animal Animal { get; set; }

        public int FoodId { get; set; }
        public required Food Food { get; set; }
        
        public DateTime FeedingTime { get; set; }
    }
}