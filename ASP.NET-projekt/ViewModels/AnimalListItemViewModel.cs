using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.ViewModels
{
    public class AnimalListItemViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Species { get; set; }
        public DietType Diet { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string EnclosureName { get; set; }
        public required string ZookeeperName { get; set; }
        public int MedicalRecordsCount { get; set; }

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
    }
}
