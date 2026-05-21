namespace ASP.NET_projekt.ViewModels
{
    public class EnclosureListItemViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public required string ZooName { get; set; }
        public int Capacity { get; set; }
        public int AnimalsCount { get; set; }
        public required string ZookeeperName { get; set; }
        public required string OccupancyStatusText { get; set; }
        public required string OccupancyStatusClass { get; set; }

        public string OccupancyDisplay => $"{AnimalsCount}/{Capacity}";
    }
}
