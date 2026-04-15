namespace ASP.NET_projekt.Models
{
    public class Enclosure
    {
        public int Id { get; set; }
        public required Zoo Zoo { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int Capacity { get; set; }

        public List<Animal> Animals { get; set; } = new ();
        public required Zookeeper Zookeeper { get; set; }
    }
}