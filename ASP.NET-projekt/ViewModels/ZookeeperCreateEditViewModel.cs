using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.ViewModels
{
    public class ZookeeperCreateEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 100 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 100 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Years of experience is required")]
        [Range(0, 60, ErrorMessage = "Years of experience must be between 0 and 60")]
        public int YearsOfExperience { get; set; }

        [Required(ErrorMessage = "Date of employment is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfEmployment { get; set; }
    }
}
