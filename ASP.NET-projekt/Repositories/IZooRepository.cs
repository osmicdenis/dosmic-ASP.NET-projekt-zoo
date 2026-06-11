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
        IEnumerable<AnimalPhoto> GetAnimalPhotosByAnimalId(int animalId);
        AnimalPhoto? GetAnimalPhotoById(int id);

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

        // CRUD Methods for Animals
        void AddAnimal(Animal animal);
        void UpdateAnimal(Animal animal);
        void DeleteAnimal(int animalId);
        void AddAnimalPhoto(AnimalPhoto animalPhoto);
        void DeleteAnimalPhoto(int animalPhotoId);

        // CRUD Methods for Zookeepers
        void AddZookeeper(Zookeeper zookeeper);
        void UpdateZookeeper(Zookeeper zookeeper);
        void DeleteZookeeper(int zookeeperId);

        // CRUD Methods for Veterinarians
        void AddVeterinarian(Veterinarian veterinarian);
        void UpdateVeterinarian(Veterinarian veterinarian);
        void DeleteVeterinarian(int veterinarianId);

        // CRUD Methods for Enclosures
        void AddEnclosure(Enclosure enclosure);
        void UpdateEnclosure(Enclosure enclosure);
        void DeleteEnclosure(int enclosureId);

        // CRUD Methods for Feedings
        void AddFeeding(Feeding feeding);
        void UpdateFeeding(Feeding feeding);
        void DeleteFeeding(int feedingId);

        // CRUD Methods for Food
        void AddFood(Food food);
        void UpdateFood(Food food);
        void DeleteFood(int foodId);

        void SaveChanges();
    }
}
