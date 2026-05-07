using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ASP.NET_projekt.Controllers
{
    public class StaffController : Controller
    {
        private readonly IZooRepository _zooRepository;

        public StaffController(IZooRepository zooRepository)
        {
            _zooRepository = zooRepository;
        }

        public IActionResult Index()
        {
            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();

            var staff = new List<StaffListItemViewModel>();

            staff.AddRange(_zooRepository.GetAllZookeepers().Select(zookeeper =>
            {
                var assignedEnclosures = enclosures.Where(e => e.Zookeeper.Id == zookeeper.Id).ToList();
                var managedAnimals = assignedEnclosures.SelectMany(e => e.Animals).DistinctBy(a => a.Id).Count();

                return new StaffListItemViewModel
                {
                    Id = zookeeper.Id,
                    Role = "Zookeeper",
                    FullName = $"{zookeeper.FirstName} {zookeeper.LastName}",
                    Subtitle = $"{zookeeper.YearsOfExperience} years of experience",
                    StatusText = assignedEnclosures.Any() ? "Actively assigned" : "No assignment",
                    StatusClass = assignedEnclosures.Any() ? "entity-chip--success" : "entity-chip--warning",
                    MetaItems = new List<KeyValuePair<string, string>>
                    {
                        new("Enclosures", assignedEnclosures.Count.ToString()),
                        new("Animals", managedAnimals.ToString()),
                        new("Employed", zookeeper.DateOfEmployment.ToString("dd MMM yyyy"))
                    }
                };
            }));

            staff.AddRange(_zooRepository.GetAllVeterinarians().Select(veterinarian =>
            {
                var recordsCount = medicalRecords.Count(r => r.Veterinarian.Id == veterinarian.Id);

                return new StaffListItemViewModel
                {
                    Id = veterinarian.Id,
                    Role = "Veterinarian",
                    FullName = $"{veterinarian.FirstName} {veterinarian.LastName}",
                    Subtitle = "Animal health specialist",
                    StatusText = recordsCount > 0 ? "Recent case history" : "No records yet",
                    StatusClass = recordsCount > 0 ? "entity-chip--success" : "entity-chip--warning",
                    MetaItems = new List<KeyValuePair<string, string>>
                    {
                        new("Medical records", recordsCount.ToString()),
                        new("Specialty", "General care")
                    }
                };
            }));

            var orderedStaff = staff.OrderBy(s => s.FullName).ToList();
            return View(orderedStaff);
        }

        public IActionResult Details(string role, int id)
        {
            var normalizedRole = role?.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalizedRole))
            {
                return NotFound();
            }

            var enclosures = _zooRepository.GetAllEnclosures().ToList();
            var medicalRecords = _zooRepository.GetAllMedicalRecords().ToList();

            if (normalizedRole == "zookeeper")
            {
                var zookeeper = _zooRepository.GetZookeeperById(id);
                if (zookeeper == null)
                {
                    return NotFound();
                }

                var assignedEnclosures = enclosures.Where(e => e.Zookeeper.Id == id).ToList();
                var assignedAnimals = assignedEnclosures.SelectMany(e => e.Animals).DistinctBy(a => a.Id).OrderBy(a => a.Name).ToList();

                var model = new StaffDetailsViewModel
                {
                    Id = zookeeper.Id,
                    Role = "Zookeeper",
                    FullName = $"{zookeeper.FirstName} {zookeeper.LastName}",
                    Subtitle = $"{zookeeper.YearsOfExperience} years of experience",
                    StatusText = assignedEnclosures.Any() ? "Actively assigned" : "No assignment",
                    StatusClass = assignedEnclosures.Any() ? "entity-chip--success" : "entity-chip--warning",
                    SnapshotItems = new List<KeyValuePair<string, string>>
                    {
                        new("Role", "Zookeeper"),
                        new("Years of experience", zookeeper.YearsOfExperience.ToString()),
                        new("Date of employment", zookeeper.DateOfEmployment.ToString("dd MMM yyyy")),
                        new("Managed enclosures", assignedEnclosures.Count.ToString()),
                        new("Managed animals", assignedAnimals.Count.ToString())
                    },
                    AssignedAnimals = assignedAnimals,
                    AssignedEnclosures = assignedEnclosures,
                    MedicalRecords = Enumerable.Empty<MedicalRecord>()
                };

                return View(model);
            }

            if (normalizedRole == "veterinarian")
            {
                var veterinarian = _zooRepository.GetVeterinarianById(id);
                if (veterinarian == null)
                {
                    return NotFound();
                }

                var vetRecords = medicalRecords
                    .Where(r => r.Veterinarian.Id == id)
                    .OrderByDescending(r => r.ExaminationDate)
                    .ToList();

                var uniqueAnimals = vetRecords.Select(r => r.Animal).DistinctBy(a => a.Id).OrderBy(a => a.Name).ToList();

                var model = new StaffDetailsViewModel
                {
                    Id = veterinarian.Id,
                    Role = "Veterinarian",
                    FullName = $"{veterinarian.FirstName} {veterinarian.LastName}",
                    Subtitle = "Animal health specialist",
                    StatusText = vetRecords.Any() ? "Recent case history" : "No records yet",
                    StatusClass = vetRecords.Any() ? "entity-chip--success" : "entity-chip--warning",
                    SnapshotItems = new List<KeyValuePair<string, string>>
                    {
                        new("Role", "Veterinarian"),
                        new("Medical records", vetRecords.Count.ToString()),
                        new("Animals treated", uniqueAnimals.Count.ToString()),
                        new("Specialty", "General care")
                    },
                    AssignedAnimals = uniqueAnimals,
                    AssignedEnclosures = Enumerable.Empty<Enclosure>(),
                    MedicalRecords = vetRecords
                };

                return View(model);
            }

            return NotFound();
        }
    }
}
