using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.Repositories
{
    public interface IZooRepository
    {
        // Zoo Methods
        IEnumerable<Zoo> GetAllZoos();
        Zoo? GetZooById(int id);

        // Enclosure Methods
        IEnumerable<Enclosure> GetAllEnclosures();
        Enclosure? GetEnclosureById(int id);
        IEnumerable<Enclosure> GetEnclosuresByZooId(int zooId);

        // Animal Methods
        IEnumerable<Animal> GetAllAnimals();
        Animal? GetAnimalById(int id);
        IEnumerable<Animal> GetAnimalsByEnclosureId(int enclosureId);
        IEnumerable<Animal> GetAnimalsByDiet(DietType diet);

        // Zookeeper Methods
        IEnumerable<Zookeeper> GetAllZookeepers();
        Zookeeper? GetZookeeperById(int id);

        // Veterinarian Methods
        IEnumerable<Veterinarian> GetAllVeterinarians();
        Veterinarian? GetVeterinarianById(int id);

        // Medical Record Methods
        IEnumerable<MedicalRecord> GetAllMedicalRecords();
        MedicalRecord? GetMedicalRecordById(int id);
        IEnumerable<MedicalRecord> GetMedicalRecordsByAnimalId(int animalId);

        // Food Methods
        IEnumerable<Food> GetAllFoods();
        Food? GetFoodById(int id);

        // Feeding Methods
        IEnumerable<Feeding> GetAllFeedings();
        Feeding? GetFeedingById(int id);
        IEnumerable<Feeding> GetFeedingsByAnimalId(int animalId);
    }
}
