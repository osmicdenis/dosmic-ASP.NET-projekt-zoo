namespace ASP.NET_projekt.ViewModels
{
    public class ZooMapViewModel
    {
        public required string MapImagePath { get; set; }
        public required List<ZooMapHotspotViewModel> Hotspots { get; set; }
    }
}
