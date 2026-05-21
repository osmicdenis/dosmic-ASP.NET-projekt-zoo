using ASP.NET_projekt.Data;
using ASP.NET_projekt.Models;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_projekt.Repositories
{
    public class EfZooRepository : IZooRepository
    {
        private readonly ZooDbContext _dbContext;

        public EfZooRepository(ZooDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zoo Methods
        public IEnumerable<Zoo> GetAllZoos()
        {
            return _dbContext.Zoos
                .Include(z => z.Enclosures)
                .ToList();
        }

        public Zoo? GetZooById(int id)
        {
            return _dbContext.Zoos
                .Include(z => z.Enclosures)
                    .ThenInclude(e => e.Animals)
                .FirstOrDefault(z => z.Id == id);
        }

        // Enclosure Methods
        public IEnumerable<Enclosure> GetAllEnclosures()
        {
            return _dbContext.Enclosures
                .Include(e => e.Zoo)
                .Include(e => e.Zookeeper)
                .Include(e => e.Animals)
                .ToList();
        }

        public Enclosure? GetEnclosureById(int id)
        {
            return _dbContext.Enclosures
                .Include(e => e.Zoo)
                .Include(e => e.Zookeeper)
                .Include(e => e.Animals)
                    .ThenInclude(a => a.MedicalRecords)
                .FirstOrDefault(e => e.Id == id);
        }

        public IEnumerable<Enclosure> GetEnclosuresByZooId(int zooId)
        {
            return _dbContext.Enclosures
                .Where(e => e.ZooId == zooId)
                .Include(e => e.Zoo)
                .Include(e => e.Zookeeper)
                .Include(e => e.Animals)
                .ToList();
        }

        // Animal Methods
        public IEnumerable<Animal> GetAllAnimals()
        {
            return _dbContext.Animals
                .Include(a => a.Enclosure)
                .Include(a => a.MedicalRecords)
                .Include(a => a.Feedings)
                .ToList();
        }

        public Animal? GetAnimalById(int id)
        {
            return _dbContext.Animals
                .Include(a => a.Enclosure)
                .Include(a => a.MedicalRecords)
                    .ThenInclude(m => m.Veterinarian)
                .Include(a => a.Feedings)
                    .ThenInclude(f => f.Food)
                .FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Animal> GetAnimalsByEnclosureId(int enclosureId)
        {
            return _dbContext.Animals
                .Where(a => a.EnclosureId == enclosureId)
                .Include(a => a.Enclosure)
                .Include(a => a.MedicalRecords)
                .Include(a => a.Feedings)
                .ToList();
        }

        public IEnumerable<Animal> GetAnimalsByDiet(DietType diet)
        {
            return _dbContext.Animals
                .Where(a => a.Diet == diet)
                .Include(a => a.Enclosure)
                .Include(a => a.MedicalRecords)
                .Include(a => a.Feedings)
                .ToList();
        }

        // Zookeeper Methods
        public IEnumerable<Zookeeper> GetAllZookeepers()
        {
            return _dbContext.Zookeepers
                .Include(z => z.Enclosures)
                .ToList();
        }

        public Zookeeper? GetZookeeperById(int id)
        {
            return _dbContext.Zookeepers
                .Include(z => z.Enclosures)
                .FirstOrDefault(z => z.Id == id);
        }

        // Veterinarian Methods
        public IEnumerable<Veterinarian> GetAllVeterinarians()
        {
            return _dbContext.Veterinarians
                .Include(v => v.MedicalRecords)
                .ToList();
        }

        public Veterinarian? GetVeterinarianById(int id)
        {
            return _dbContext.Veterinarians
                .Include(v => v.MedicalRecords)
                .FirstOrDefault(v => v.Id == id);
        }

        // Medical Record Methods
        public IEnumerable<MedicalRecord> GetAllMedicalRecords()
        {
            return _dbContext.MedicalRecords
                .Include(m => m.Animal)
                .Include(m => m.Veterinarian)
                .ToList();
        }

        public MedicalRecord? GetMedicalRecordById(int id)
        {
            return _dbContext.MedicalRecords
                .Include(m => m.Animal)
                .Include(m => m.Veterinarian)
                .FirstOrDefault(m => m.Id == id);
        }

        public IEnumerable<MedicalRecord> GetMedicalRecordsByAnimalId(int animalId)
        {
            return _dbContext.MedicalRecords
                .Where(m => m.AnimalId == animalId)
                .Include(m => m.Animal)
                .Include(m => m.Veterinarian)
                .ToList();
        }

        // Food Methods
        public IEnumerable<Food> GetAllFoods()
        {
            return _dbContext.Foods
                .Include(f => f.Feedings)
                .ToList();
        }

        public Food? GetFoodById(int id)
        {
            return _dbContext.Foods
                .Include(f => f.Feedings)
                .FirstOrDefault(f => f.Id == id);
        }

        // Feeding Methods
        public IEnumerable<Feeding> GetAllFeedings()
        {
            return _dbContext.Feedings
                .Include(f => f.Animal)
                .Include(f => f.Food)
                .ToList();
        }

        public Feeding? GetFeedingById(int id)
        {
            return _dbContext.Feedings
                .Include(f => f.Animal)
                .Include(f => f.Food)
                .FirstOrDefault(f => f.Id == id);
        }

        public IEnumerable<Feeding> GetFeedingsByAnimalId(int animalId)
        {
            return _dbContext.Feedings
                .Where(f => f.AnimalId == animalId)
                .Include(f => f.Animal)
                .Include(f => f.Food)
                .ToList();
        }

        // CRUD Methods for Animals
        public void AddAnimal(Animal animal)
        {
            _dbContext.Animals.Add(animal);
        }

        public void UpdateAnimal(Animal animal)
        {
            _dbContext.Animals.Update(animal);
        }

        public void DeleteAnimal(int animalId)
        {
            var animal = _dbContext.Animals.FirstOrDefault(a => a.Id == animalId);
            if (animal != null)
            {
                _dbContext.Animals.Remove(animal);
            }
        }

        // CRUD Methods for Zookeepers
        public void AddZookeeper(Zookeeper zookeeper)
        {
            _dbContext.Zookeepers.Add(zookeeper);
        }

        public void UpdateZookeeper(Zookeeper zookeeper)
        {
            _dbContext.Zookeepers.Update(zookeeper);
        }

        public void DeleteZookeeper(int zookeeperId)
        {
            var zookeeper = _dbContext.Zookeepers.FirstOrDefault(z => z.Id == zookeeperId);
            if (zookeeper != null)
            {
                _dbContext.Zookeepers.Remove(zookeeper);
            }
        }

        // CRUD Methods for Veterinarians
        public void AddVeterinarian(Veterinarian veterinarian)
        {
            _dbContext.Veterinarians.Add(veterinarian);
        }

        public void UpdateVeterinarian(Veterinarian veterinarian)
        {
            _dbContext.Veterinarians.Update(veterinarian);
        }

        public void DeleteVeterinarian(int veterinarianId)
        {
            var veterinarian = _dbContext.Veterinarians.FirstOrDefault(v => v.Id == veterinarianId);
            if (veterinarian != null)
            {
                _dbContext.Veterinarians.Remove(veterinarian);
            }
        }

        // CRUD Methods for Enclosures
        public void AddEnclosure(Enclosure enclosure)
        {
            _dbContext.Enclosures.Add(enclosure);
        }

        public void UpdateEnclosure(Enclosure enclosure)
        {
            _dbContext.Enclosures.Update(enclosure);
        }

        public void DeleteEnclosure(int enclosureId)
        {
            var enclosure = _dbContext.Enclosures.FirstOrDefault(e => e.Id == enclosureId);
            if (enclosure != null)
            {
                _dbContext.Enclosures.Remove(enclosure);
            }
        }

        // CRUD Methods for Feedings
        public void AddFeeding(Feeding feeding)
        {
            _dbContext.Feedings.Add(feeding);
        }

        public void UpdateFeeding(Feeding feeding)
        {
            _dbContext.Feedings.Update(feeding);
        }

        public void DeleteFeeding(int feedingId)
        {
            var feeding = _dbContext.Feedings.FirstOrDefault(f => f.Id == feedingId);
            if (feeding != null)
            {
                _dbContext.Feedings.Remove(feeding);
            }
        }

        // CRUD Methods for Food
        public void AddFood(Food food)
        {
            _dbContext.Foods.Add(food);
        }

        public void UpdateFood(Food food)
        {
            _dbContext.Foods.Update(food);
        }

        public void DeleteFood(int foodId)
        {
            var food = _dbContext.Foods.FirstOrDefault(f => f.Id == foodId);
            if (food != null)
            {
                _dbContext.Foods.Remove(food);
            }
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}
