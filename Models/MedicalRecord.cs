namespace ASP.NET_projekt.Models
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public required string Diagnosis { get; set; }
        public required string Therapy { get; set; }
        public DateTime ExaminationDate { get; set; }
        public required Veterinarian Veterinarian { get; set; }
        public required Animal Animal { get; set; }
    }
}