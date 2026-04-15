namespace ASP.NET_projekt.ViewModels
{
    public class ZooMapHotspotViewModel
    {
        public int EnclosureId { get; set; }
        public required string EnclosureName { get; set; }
        public required string EnclosureType { get; set; }
        public required string ZookeeperName { get; set; }
        public int Capacity { get; set; }
        public int AnimalCount { get; set; }
        public required string OccupancyDisplay { get; set; }
        public required string OccupancyStatusText { get; set; }
        public required string OccupancyStatusClass { get; set; }
        public double LeftPercent { get; set; }
        public double TopPercent { get; set; }
        public required List<string> AnimalSummaries { get; set; }
    }
}
