using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class StaffDetailsViewModel
    {
        public int Id { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
        public required string Subtitle { get; set; }
        public required string StatusText { get; set; }
        public required string StatusClass { get; set; }
        public required List<KeyValuePair<string, string>> SnapshotItems { get; set; }

        public required IEnumerable<Animal> AssignedAnimals { get; set; }
        public required IEnumerable<Enclosure> AssignedEnclosures { get; set; }
        public required IEnumerable<MedicalRecord> MedicalRecords { get; set; }
    }
}
