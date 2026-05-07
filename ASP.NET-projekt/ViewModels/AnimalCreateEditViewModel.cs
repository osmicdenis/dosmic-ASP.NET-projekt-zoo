using ASP.NET_projekt.Models;
using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.ViewModels
{
    public class AnimalCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Animal name is required")]
        [StringLength(100, MinimumLength = 2, 
            ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Species is required")]
        [StringLength(100, MinimumLength = 2, 
            ErrorMessage = "Species must be between 2 and 100 characters")]
        public string Species { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Date of arrival is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Arrival")]
        public DateTime DateOfArrival { get; set; }

        [Required(ErrorMessage = "Diet type is required")]
        [Display(Name = "Diet Type")]
        public DietType Diet { get; set; }

        [Display(Name = "Enclosure")]
        public int? EnclosureId { get; set; }

        public List<EnclosureOption> AvailableEnclosures { get; set; } = new();
    }

    public class EnclosureOption
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
