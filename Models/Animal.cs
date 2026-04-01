namespace ASP.NET_projekt.Models
{
    public class Animal
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime DateOfArrival { get; set; }
        public DietType Diet { get; set; }
        
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }
}
