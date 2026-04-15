using ASP.NET_projekt.Models;

namespace ASP.NET_projekt.Repositories
{
    public class MockZooRepository : IZooRepository
    {
        private static List<Zoo>? _zoos;
        private static List<Zookeeper>? _zookeepers;
        private static List<Veterinarian>? _veterinarians;
        private static List<Enclosure>? _enclosures;
        private static List<Animal>? _animals;
        private static List<MedicalRecord>? _medicalRecords;
        private static List<Food>? _foods;
        private static List<Feeding>? _feedings;

        static MockZooRepository()
        {
            InitializeData();
        }

        public MockZooRepository()
        {
            // Data is already initialized in static constructor
        }

        private static void InitializeData()
        {
            // Initialize Zoos
            _zoos = new List<Zoo>
            {
                new Zoo { Id = 1, Name = "Zoo Zagreb", Location = "Maksimir" }
            };

            // Initialize Zookeepers
            _zookeepers = new List<Zookeeper>
            {
                new Zookeeper { Id = 1, FirstName = "Denis", LastName = "Osmić", YearsOfExperience = 10, DateOfEmployment = new DateTime(2015, 4, 1) },
                new Zookeeper { Id = 2, FirstName = "Danijel", LastName = "Osmić", YearsOfExperience = 5, DateOfEmployment = new DateTime(2018, 9, 15) },
                new Zookeeper { Id = 3, FirstName = "Marko", LastName = "Horvat", YearsOfExperience = 8, DateOfEmployment = new DateTime(2016, 6, 20) }
            };

            // Initialize Veterinarians
            _veterinarians = new List<Veterinarian>
            {
                new Veterinarian { Id = 1, FirstName = "Ivan", LastName = "Horvat" },
                new Veterinarian { Id = 2, FirstName = "Ana", LastName = "Jurić" }
            };

            // Initialize Animals (will be populated with enclosures)
            _animals = new List<Animal>
            {
                new Animal { Id = 1, Name = "Simba", Species = "Lion", DateOfBirth = new DateTime(2019, 5, 12), DateOfArrival = new DateTime(2020, 2, 20), Diet = DietType.Carnivore },
                new Animal { Id = 2, Name = "Nala", Species = "Lion", DateOfBirth = new DateTime(2020, 8, 22), DateOfArrival = new DateTime(2021, 3, 10), Diet = DietType.Carnivore },
                new Animal { Id = 3, Name = "Koko", Species = "Gorilla", DateOfBirth = new DateTime(2018, 11, 3), DateOfArrival = new DateTime(2020, 6, 5), Diet = DietType.Omnivore },
                new Animal { Id = 4, Name = "Rex", Species = "Crocodile", DateOfBirth = new DateTime(2010, 4, 18), DateOfArrival = new DateTime(2012, 9, 30), Diet = DietType.Carnivore },
                new Animal { Id = 5, Name = "Elefant", Species = "African Elephant", DateOfBirth = new DateTime(2015, 3, 7), DateOfArrival = new DateTime(2019, 8, 12), Diet = DietType.Herbivore },
                new Animal { Id = 6, Name = "Žirafa", Species = "Giraffe", DateOfBirth = new DateTime(2018, 1, 20), DateOfArrival = new DateTime(2020, 5, 15), Diet = DietType.Herbivore },
                new Animal { Id = 7, Name = "Papagaj", Species = "Parrot", DateOfBirth = new DateTime(2017, 7, 14), DateOfArrival = new DateTime(2019, 11, 10), Diet = DietType.Omnivore }
            };

            // Initialize Enclosures (one dedicated enclosure per animal species)
            _enclosures = new List<Enclosure>
            {
                new Enclosure
                {
                    Id = 1,
                    Zoo = _zoos[0],
                    Name = "Lion Rock",
                    Type = "Savanna Habitat",
                    Capacity = 4,
                    Zookeeper = _zookeepers[0],
                    Animals = new List<Animal> { _animals[0], _animals[1] }
                },
                new Enclosure
                {
                    Id = 2,
                    Zoo = _zoos[0],
                    Name = "Gorilla Grove",
                    Type = "Dense Forest",
                    Capacity = 4,
                    Zookeeper = _zookeepers[1],
                    Animals = new List<Animal> { _animals[2] }
                },
                new Enclosure
                {
                    Id = 3,
                    Zoo = _zoos[0],
                    Name = "Reptile House",
                    Type = "Indoor",
                    Capacity = 10,
                    Zookeeper = _zookeepers[0],
                    Animals = new List<Animal> { _animals[3] }
                },
                new Enclosure
                {
                    Id = 4,
                    Zoo = _zoos[0],
                    Name = "Elephant Plains",
                    Type = "Open Grassland",
                    Capacity = 3,
                    Zookeeper = _zookeepers[2],
                    Animals = new List<Animal> { _animals[4] }
                },
                new Enclosure
                {
                    Id = 5,
                    Zoo = _zoos[0],
                    Name = "Giraffe Terrace",
                    Type = "Open Grassland",
                    Capacity = 3,
                    Zookeeper = _zookeepers[2],
                    Animals = new List<Animal> { _animals[5] }
                },
                new Enclosure
                {
                    Id = 6,
                    Zoo = _zoos[0],
                    Name = "Parrot Aviary",
                    Type = "Aviary",
                    Capacity = 12,
                    Zookeeper = _zookeepers[1],
                    Animals = new List<Animal> { _animals[6] }
                }
            };

            // Initialize Foods
            _foods = new List<Food>
            {
                new Food { Id = 1, Name = "Fresh Meat" },
                new Food { Id = 2, Name = "Mixed Fruit" },
                new Food { Id = 3, Name = "Leaf Bundle" },
                new Food { Id = 4, Name = "Fish Portion" },
                new Food { Id = 5, Name = "Seed Mix" }
            };

            // Initialize Feedings
            _feedings = new List<Feeding>
            {
                new Feeding { Id = 1, AnimalId = _animals[0].Id, Animal = _animals[0], FoodId = _foods[0].Id, Food = _foods[0], FeedingTime = new DateTime(2026, 4, 13, 8, 0, 0) },
                new Feeding { Id = 2, AnimalId = _animals[1].Id, Animal = _animals[1], FoodId = _foods[0].Id, Food = _foods[0], FeedingTime = new DateTime(2026, 4, 13, 8, 30, 0) },
                new Feeding { Id = 3, AnimalId = _animals[2].Id, Animal = _animals[2], FoodId = _foods[1].Id, Food = _foods[1], FeedingTime = new DateTime(2026, 4, 13, 10, 0, 0) },
                new Feeding { Id = 4, AnimalId = _animals[3].Id, Animal = _animals[3], FoodId = _foods[3].Id, Food = _foods[3], FeedingTime = new DateTime(2026, 4, 13, 11, 15, 0) },
                new Feeding { Id = 5, AnimalId = _animals[4].Id, Animal = _animals[4], FoodId = _foods[2].Id, Food = _foods[2], FeedingTime = new DateTime(2026, 4, 13, 9, 45, 0) },
                new Feeding { Id = 6, AnimalId = _animals[5].Id, Animal = _animals[5], FoodId = _foods[2].Id, Food = _foods[2], FeedingTime = new DateTime(2026, 4, 13, 10, 30, 0) },
                new Feeding { Id = 7, AnimalId = _animals[6].Id, Animal = _animals[6], FoodId = _foods[4].Id, Food = _foods[4], FeedingTime = new DateTime(2026, 4, 13, 12, 0, 0) }
            };

            // Initialize Medical Records
            _medicalRecords = new List<MedicalRecord>
            {
                new MedicalRecord
                {
                    Id = 1,
                    Diagnosis = "Mild respiratory infection",
                    Therapy = "Antibiotics and rest",
                    ExaminationDate = new DateTime(2025, 3, 15, 10, 30, 0),
                    Veterinarian = _veterinarians[0],
                    Animal = _animals[2] // Koko
                },
                new MedicalRecord
                {
                    Id = 2,
                    Diagnosis = "Routine checkup",
                    Therapy = "Vaccination",
                    ExaminationDate = new DateTime(2025, 2, 20, 14, 0, 0),
                    Veterinarian = _veterinarians[1],
                    Animal = _animals[0] // Simba
                },
                new MedicalRecord
                {
                    Id = 3,
                    Diagnosis = "Skin infection",
                    Therapy = "Topical treatment",
                    ExaminationDate = new DateTime(2025, 1, 10, 9, 30, 0),
                    Veterinarian = _veterinarians[0],
                    Animal = _animals[3] // Rex
                }
            };

            // Add medical records to animals
            _animals[2].MedicalRecords.Add(_medicalRecords[0]); // Koko
            _animals[0].MedicalRecords.Add(_medicalRecords[1]); // Simba
            _animals[3].MedicalRecords.Add(_medicalRecords[2]); // Rex
        }

        // Zoo Methods
        public IEnumerable<Zoo> GetAllZoos() => _zoos?.AsEnumerable() ?? Enumerable.Empty<Zoo>();
        public Zoo? GetZooById(int id) => _zoos?.FirstOrDefault(z => z.Id == id);

        // Enclosure Methods
        public IEnumerable<Enclosure> GetAllEnclosures() => _enclosures?.AsEnumerable() ?? Enumerable.Empty<Enclosure>();
        public Enclosure? GetEnclosureById(int id) => _enclosures?.FirstOrDefault(e => e.Id == id);
        public IEnumerable<Enclosure> GetEnclosuresByZooId(int zooId) => _enclosures?.Where(e => e.Zoo.Id == zooId) ?? Enumerable.Empty<Enclosure>();

        // Animal Methods
        public IEnumerable<Animal> GetAllAnimals() => _animals?.AsEnumerable() ?? Enumerable.Empty<Animal>();
        public Animal? GetAnimalById(int id) => _animals?.FirstOrDefault(a => a.Id == id);
        public IEnumerable<Animal> GetAnimalsByEnclosureId(int enclosureId)
        {
            var enclosure = GetEnclosureById(enclosureId);
            return enclosure?.Animals ?? Enumerable.Empty<Animal>();
        }
        public IEnumerable<Animal> GetAnimalsByDiet(DietType diet) => _animals?.Where(a => a.Diet == diet) ?? Enumerable.Empty<Animal>();

        // Zookeeper Methods
        public IEnumerable<Zookeeper> GetAllZookeepers() => _zookeepers?.AsEnumerable() ?? Enumerable.Empty<Zookeeper>();
        public Zookeeper? GetZookeeperById(int id) => _zookeepers?.FirstOrDefault(z => z.Id == id);

        // Veterinarian Methods
        public IEnumerable<Veterinarian> GetAllVeterinarians() => _veterinarians?.AsEnumerable() ?? Enumerable.Empty<Veterinarian>();
        public Veterinarian? GetVeterinarianById(int id) => _veterinarians?.FirstOrDefault(v => v.Id == id);

        // Medical Record Methods
        public IEnumerable<MedicalRecord> GetAllMedicalRecords() => _medicalRecords?.AsEnumerable() ?? Enumerable.Empty<MedicalRecord>();
        public MedicalRecord? GetMedicalRecordById(int id) => _medicalRecords?.FirstOrDefault(m => m.Id == id);
        public IEnumerable<MedicalRecord> GetMedicalRecordsByAnimalId(int animalId) => _medicalRecords?.Where(m => m.Animal.Id == animalId) ?? Enumerable.Empty<MedicalRecord>();

        // Food Methods
        public IEnumerable<Food> GetAllFoods() => _foods?.AsEnumerable() ?? Enumerable.Empty<Food>();
        public Food? GetFoodById(int id) => _foods?.FirstOrDefault(f => f.Id == id);

        // Feeding Methods
        public IEnumerable<Feeding> GetAllFeedings() => _feedings?.AsEnumerable() ?? Enumerable.Empty<Feeding>();
        public Feeding? GetFeedingById(int id) => _feedings?.FirstOrDefault(f => f.Id == id);
        public IEnumerable<Feeding> GetFeedingsByAnimalId(int animalId) => _feedings?.Where(f => f.AnimalId == animalId) ?? Enumerable.Empty<Feeding>();
    }
}
