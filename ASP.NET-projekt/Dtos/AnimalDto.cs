using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.Dtos
{
    public class AnimalDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Species { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime DateOfArrival { get; set; }

        public DietType Diet { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;

                if (DateOfBirth.Date > today.AddYears(-age))
                {
                    age--;
                }

                return age;
            }
        }

        public EnclosureSummaryDto? Enclosure { get; set; }

        public int MedicalRecordsCount { get; set; }

        public int FeedingsCount { get; set; }
    }
}