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
            return View(GetStaffListItems(null));
        }

        [HttpGet]
        public IActionResult Search(string? query)
        {
            var staff = GetStaffListItems(query)
                .Select(item => new
                {
                    id = item.Id,
                    role = item.Role,
                    fullName = item.FullName,
                    subtitle = item.Subtitle,
                    statusText = item.StatusText,
                    statusClass = item.StatusClass,
                    metaItems = item.MetaItems.Select(meta => new { key = meta.Key, value = meta.Value })
                })
                .ToList();

            return Json(staff);
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

        private List<StaffListItemViewModel> GetStaffListItems(string? search)
        {
            var normalizedSearch = search?.Trim();
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

            return staff
                .Where(item => string.IsNullOrWhiteSpace(normalizedSearch) ||
                               item.FullName.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.Role.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase) ||
                               item.Subtitle.Contains(normalizedSearch, StringComparison.OrdinalIgnoreCase))
                .OrderBy(item => item.FullName)
                .ToList();
        }

        // Zookeeper CRUD
        [HttpGet]
        public IActionResult CreateZookeeper()
        {
            var viewModel = new ZookeeperCreateEditViewModel
            {
                DateOfEmployment = DateTime.Now
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateZookeeper(ZookeeperCreateEditViewModel viewModel)
        {
            // Additional server-side validation
            ValidateZookeeperViewModel(viewModel);

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var zookeeper = new Zookeeper
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                YearsOfExperience = viewModel.YearsOfExperience,
                DateOfEmployment = viewModel.DateOfEmployment
            };

            _zooRepository.AddZookeeper(zookeeper);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { role = "zookeeper", id = zookeeper.Id });
        }

        [HttpGet]
        public IActionResult EditZookeeper(int? id)
        {
            if (id == null)
                return NotFound();


            var zookeeper = _zooRepository.GetZookeeperById(id.Value);
            if (zookeeper == null)
                return NotFound();

            var viewModel = new ZookeeperCreateEditViewModel
            {
                Id = zookeeper.Id,
                FirstName = zookeeper.FirstName,
                LastName = zookeeper.LastName,
                YearsOfExperience = zookeeper.YearsOfExperience,
                DateOfEmployment = zookeeper.DateOfEmployment
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditZookeeper(int id, ZookeeperCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var zookeeper = _zooRepository.GetZookeeperById(id);
            if (zookeeper == null)
                return NotFound();

            zookeeper.FirstName = viewModel.FirstName;
            zookeeper.LastName = viewModel.LastName;
            zookeeper.YearsOfExperience = viewModel.YearsOfExperience;
            zookeeper.DateOfEmployment = viewModel.DateOfEmployment;

            _zooRepository.UpdateZookeeper(zookeeper);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { role = "zookeeper", id = id });
        }

        [HttpGet]
        public IActionResult DeleteZookeeper(int? id)
        {
            if (id == null)
                return NotFound();

            var zookeeper = _zooRepository.GetZookeeperById(id.Value);
            if (zookeeper == null)
                return NotFound();

            var enclosures = _zooRepository.GetAllEnclosures().Where(e => e.ZookeeperId == id).ToList();

            var viewModel = new ZookeeperCreateEditViewModel
            {
                Id = zookeeper.Id,
                FirstName = zookeeper.FirstName,
                LastName = zookeeper.LastName,
                YearsOfExperience = zookeeper.YearsOfExperience,
                DateOfEmployment = zookeeper.DateOfEmployment
            };
            ViewBag.EnclosureCount = enclosures.Count;
            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteZookeeper")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteZookeeperConfirmed(int id)
        {
            var zookeeper = _zooRepository.GetZookeeperById(id);
            if (zookeeper == null)
                return NotFound();

            var assignedEnclosuresCount = _zooRepository.GetAllEnclosures().Count(e => e.ZookeeperId == id);
            if (assignedEnclosuresCount > 0)
            {
                ModelState.AddModelError(string.Empty, "This zookeeper cannot be deleted because they are assigned to one or more enclosures.");

                var viewModel = new ZookeeperCreateEditViewModel
                {
                    Id = zookeeper.Id,
                    FirstName = zookeeper.FirstName,
                    LastName = zookeeper.LastName,
                    YearsOfExperience = zookeeper.YearsOfExperience,
                    DateOfEmployment = zookeeper.DateOfEmployment
                };

                ViewBag.EnclosureCount = assignedEnclosuresCount;
                return View("DeleteZookeeper", viewModel);
            }

            _zooRepository.DeleteZookeeper(id);
            _zooRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        // Veterinarian CRUD
        [HttpGet]
        public IActionResult CreateVeterinarian()
        {
            var viewModel = new VeterinarianCreateEditViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateVeterinarian(VeterinarianCreateEditViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var veterinarian = new Veterinarian
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName
            };

            _zooRepository.AddVeterinarian(veterinarian);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { role = "veterinarian", id = veterinarian.Id });
        }

        [HttpGet]
        public IActionResult EditVeterinarian(int? id)
        {
            if (id == null)
                return NotFound();

            var veterinarian = _zooRepository.GetVeterinarianById(id.Value);
            if (veterinarian == null)
                return NotFound();

            var viewModel = new VeterinarianCreateEditViewModel
            {
                Id = veterinarian.Id,
                FirstName = veterinarian.FirstName,
                LastName = veterinarian.LastName
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditVeterinarian(int id, VeterinarianCreateEditViewModel viewModel)
        {
            if (id != viewModel.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var veterinarian = _zooRepository.GetVeterinarianById(id);
            if (veterinarian == null)
                return NotFound();

            veterinarian.FirstName = viewModel.FirstName;
            veterinarian.LastName = viewModel.LastName;

            _zooRepository.UpdateVeterinarian(veterinarian);
            _zooRepository.SaveChanges();

            return RedirectToAction("Details", new { role = "veterinarian", id = id });
        }

        [HttpGet]
        public IActionResult DeleteVeterinarian(int? id)
        {
            if (id == null)
                return NotFound();

            var veterinarian = _zooRepository.GetVeterinarianById(id.Value);
            if (veterinarian == null)
                return NotFound();

            var medicalRecords = _zooRepository.GetAllMedicalRecords().Where(m => m.Veterinarian.Id == id).ToList();

            var viewModel = new VeterinarianCreateEditViewModel
            {
                Id = veterinarian.Id,
                FirstName = veterinarian.FirstName,
                LastName = veterinarian.LastName
            };
            ViewBag.RecordCount = medicalRecords.Count;
            return View(viewModel);
        }

        [HttpPost, ActionName("DeleteVeterinarian")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteVeterinarianConfirmed(int id)
        {
            var veterinarian = _zooRepository.GetVeterinarianById(id);
            if (veterinarian == null)
                return NotFound();

            _zooRepository.DeleteVeterinarian(id);
            _zooRepository.SaveChanges();

            return RedirectToAction("Index");
        }

        private void ValidateZookeeperViewModel(ZookeeperCreateEditViewModel vm)
        {
            // Date of employment should not be in the future
            if (vm.DateOfEmployment > DateTime.Now)
            {
                ModelState.AddModelError(nameof(vm.DateOfEmployment), "Date of employment cannot be in the future.");
            }
        }
    }
}
