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
        private static List<AnimalPhoto>? _animalPhotos;

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

            _animalPhotos = new List<AnimalPhoto>
            {
                new AnimalPhoto
                {
                    Id = 1,
                    AnimalId = _animals[0].Id,
                    Animal = _animals[0],
                    FileName = "simba.jpg",
                    FilePath = "/uploads/animals/1/simba.jpg",
                    ContentType = "image/jpeg",
                    FileSize = 256_000,
                    CreatedAt = new DateTime(2026, 4, 13, 13, 0, 0)
                }
            };

            _animals[0].Photos.Add(_animalPhotos[0]);
        }

        // Zoo Methods
        public IEnumerable<Zoo> GetAllZoos() => _zoos?.AsEnumerable() ?? Enumerable.Empty<Zoo>();
        public Zoo? GetZooById(int id) => _zoos?.FirstOrDefault(z => z.Id == id);

        // Enclosure Methods
        public IEnumerable<Enclosure> GetAllEnclosures() => _enclosures?.AsEnumerable() ?? Enumerable.Empty<Enclosure>();
        public Enclosure? GetEnclosureById(int id) => _enclosures?.FirstOrDefault(e => e.Id == id);
        public IEnumerable<Enclosure> GetEnclosuresByZooId(int zooId) => _enclosures?.Where(e => e.Zoo != null && e.Zoo.Id == zooId) ?? Enumerable.Empty<Enclosure>();

        // Animal Methods
        public IEnumerable<Animal> GetAllAnimals() => _animals?.AsEnumerable() ?? Enumerable.Empty<Animal>();
        public Animal? GetAnimalById(int id) => _animals?.FirstOrDefault(a => a.Id == id);
        public IEnumerable<Animal> GetAnimalsByEnclosureId(int enclosureId)
        {
            var enclosure = GetEnclosureById(enclosureId);
            return enclosure?.Animals ?? Enumerable.Empty<Animal>();
        }
        public IEnumerable<Animal> GetAnimalsByDiet(DietType diet) => _animals?.Where(a => a.Diet == diet) ?? Enumerable.Empty<Animal>();
        public IEnumerable<AnimalPhoto> GetAnimalPhotosByAnimalId(int animalId) => _animalPhotos?.Where(photo => photo.AnimalId == animalId).OrderByDescending(photo => photo.CreatedAt) ?? Enumerable.Empty<AnimalPhoto>();
        public AnimalPhoto? GetAnimalPhotoById(int id) => _animalPhotos?.FirstOrDefault(photo => photo.Id == id);

        // Zookeeper Methods
        public IEnumerable<Zookeeper> GetAllZookeepers() => _zookeepers?.AsEnumerable() ?? Enumerable.Empty<Zookeeper>();
        public Zookeeper? GetZookeeperById(int id) => _zookeepers?.FirstOrDefault(z => z.Id == id);

        // Veterinarian Methods
        public IEnumerable<Veterinarian> GetAllVeterinarians() => _veterinarians?.AsEnumerable() ?? Enumerable.Empty<Veterinarian>();
        public Veterinarian? GetVeterinarianById(int id) => _veterinarians?.FirstOrDefault(v => v.Id == id);

        // Medical Record Methods
        public IEnumerable<MedicalRecord> GetAllMedicalRecords() => _medicalRecords?.AsEnumerable() ?? Enumerable.Empty<MedicalRecord>();
        public MedicalRecord? GetMedicalRecordById(int id) => _medicalRecords?.FirstOrDefault(m => m.Id == id);
        public IEnumerable<MedicalRecord> GetMedicalRecordsByAnimalId(int animalId) => _medicalRecords?.Where(m => m.Animal != null && m.Animal.Id == animalId) ?? Enumerable.Empty<MedicalRecord>();

        // Food Methods
        public IEnumerable<Food> GetAllFoods() => _foods?.AsEnumerable() ?? Enumerable.Empty<Food>();
        public Food? GetFoodById(int id) => _foods?.FirstOrDefault(f => f.Id == id);

        // Feeding Methods
        public IEnumerable<Feeding> GetAllFeedings() => _feedings?.AsEnumerable() ?? Enumerable.Empty<Feeding>();
        public Feeding? GetFeedingById(int id) => _feedings?.FirstOrDefault(f => f.Id == id);
        public IEnumerable<Feeding> GetFeedingsByAnimalId(int animalId) => _feedings?.Where(f => f.AnimalId == animalId) ?? Enumerable.Empty<Feeding>();

        // CRUD Methods for Animals
        public void AddAnimal(Animal animal)
        {
            if (_animals != null)
            {
                animal.Id = (_animals.Count > 0 ? _animals.Max(a => a.Id) : 0) + 1;
                _animals.Add(animal);
            }
        }

        public void UpdateAnimal(Animal animal)
        {
            if (_animals != null)
            {
                var existingAnimal = _animals.FirstOrDefault(a => a.Id == animal.Id);
                if (existingAnimal != null)
                {
                    existingAnimal.Name = animal.Name;
                    existingAnimal.Species = animal.Species;
                    existingAnimal.DateOfBirth = animal.DateOfBirth;
                    existingAnimal.DateOfArrival = animal.DateOfArrival;
                    existingAnimal.Diet = animal.Diet;
                    existingAnimal.EnclosureId = animal.EnclosureId;
                    existingAnimal.Enclosure = animal.Enclosure;
                }
            }
        }

        public void DeleteAnimal(int animalId)
        {
            if (_animals != null)
            {
                var animal = _animals.FirstOrDefault(a => a.Id == animalId);
                if (animal != null)
                {
                    _animalPhotos?.RemoveAll(photo => photo.AnimalId == animalId);
                    _animals.Remove(animal);
                }
            }
        }

        public void AddAnimalPhoto(AnimalPhoto animalPhoto)
        {
            if (_animalPhotos != null)
            {
                animalPhoto.Id = (_animalPhotos.Count > 0 ? _animalPhotos.Max(photo => photo.Id) : 0) + 1;
                _animalPhotos.Add(animalPhoto);

                var animal = _animals?.FirstOrDefault(a => a.Id == animalPhoto.AnimalId);
                if (animal != null)
                {
                    animalPhoto.Animal = animal;
                    animal.Photos.Add(animalPhoto);
                }
            }
        }

        public void DeleteAnimalPhoto(int animalPhotoId)
        {
            if (_animalPhotos != null)
            {
                var animalPhoto = _animalPhotos.FirstOrDefault(photo => photo.Id == animalPhotoId);
                if (animalPhoto != null)
                {
                    _animalPhotos.Remove(animalPhoto);

                    var animal = _animals?.FirstOrDefault(a => a.Id == animalPhoto.AnimalId);
                    animal?.Photos.Remove(animalPhoto);
                }
            }
        }

        // CRUD Methods for Zookeepers
        public void AddZookeeper(Zookeeper zookeeper)
        {
            if (_zookeepers != null)
            {
                zookeeper.Id = (_zookeepers.Count > 0 ? _zookeepers.Max(z => z.Id) : 0) + 1;
                _zookeepers.Add(zookeeper);
            }
        }

        public void UpdateZookeeper(Zookeeper zookeeper)
        {
            if (_zookeepers != null)
            {
                var existing = _zookeepers.FirstOrDefault(z => z.Id == zookeeper.Id);
                if (existing != null)
                {
                    existing.FirstName = zookeeper.FirstName;
                    existing.LastName = zookeeper.LastName;
                    existing.YearsOfExperience = zookeeper.YearsOfExperience;
                    existing.DateOfEmployment = zookeeper.DateOfEmployment;
                }
            }
        }

        public void DeleteZookeeper(int zookeeperId)
        {
            if (_zookeepers != null)
            {
                var zookeeper = _zookeepers.FirstOrDefault(z => z.Id == zookeeperId);
                if (zookeeper != null)
                {
                    _zookeepers.Remove(zookeeper);
                }
            }
        }

        // CRUD Methods for Veterinarians
        public void AddVeterinarian(Veterinarian veterinarian)
        {
            if (_veterinarians != null)
            {
                veterinarian.Id = (_veterinarians.Count > 0 ? _veterinarians.Max(v => v.Id) : 0) + 1;
                _veterinarians.Add(veterinarian);
            }
        }

        public void UpdateVeterinarian(Veterinarian veterinarian)
        {
            if (_veterinarians != null)
            {
                var existing = _veterinarians.FirstOrDefault(v => v.Id == veterinarian.Id);
                if (existing != null)
                {
                    existing.FirstName = veterinarian.FirstName;
                    existing.LastName = veterinarian.LastName;
                }
            }
        }

        public void DeleteVeterinarian(int veterinarianId)
        {
            if (_veterinarians != null)
            {
                var veterinarian = _veterinarians.FirstOrDefault(v => v.Id == veterinarianId);
                if (veterinarian != null)
                {
                    _veterinarians.Remove(veterinarian);
                }
            }
        }

        // CRUD Methods for Enclosures
        public void AddEnclosure(Enclosure enclosure)
        {
            if (_enclosures != null)
            {
                enclosure.Id = (_enclosures.Count > 0 ? _enclosures.Max(e => e.Id) : 0) + 1;
                _enclosures.Add(enclosure);
            }
        }

        public void UpdateEnclosure(Enclosure enclosure)
        {
            if (_enclosures != null)
            {
                var existing = _enclosures.FirstOrDefault(e => e.Id == enclosure.Id);
                if (existing != null)
                {
                    existing.Name = enclosure.Name;
                    existing.Type = enclosure.Type;
                    existing.Capacity = enclosure.Capacity;
                    existing.ZookeeperId = enclosure.ZookeeperId;
                    existing.Zookeeper = enclosure.Zookeeper;
                    existing.ZooId = enclosure.ZooId;
                    existing.Zoo = enclosure.Zoo;
                }
            }
        }

        public void DeleteEnclosure(int enclosureId)
        {
            if (_enclosures != null)
            {
                var enclosure = _enclosures.FirstOrDefault(e => e.Id == enclosureId);
                if (enclosure != null)
                {
                    _enclosures.Remove(enclosure);
                }
            }
        }

        // CRUD Methods for Feedings
        public void AddFeeding(Feeding feeding)
        {
            if (_feedings != null)
            {
                feeding.Id = (_feedings.Count > 0 ? _feedings.Max(f => f.Id) : 0) + 1;
                _feedings.Add(feeding);
            }
        }

        public void UpdateFeeding(Feeding feeding)
        {
            if (_feedings != null)
            {
                var existing = _feedings.FirstOrDefault(f => f.Id == feeding.Id);
                if (existing != null)
                {
                    existing.AnimalId = feeding.AnimalId;
                    existing.Animal = feeding.Animal;
                    existing.FoodId = feeding.FoodId;
                    existing.Food = feeding.Food;
                    existing.FeedingTime = feeding.FeedingTime;
                }
            }
        }

        public void DeleteFeeding(int feedingId)
        {
            if (_feedings != null)
            {
                var feeding = _feedings.FirstOrDefault(f => f.Id == feedingId);
                if (feeding != null)
                {
                    _feedings.Remove(feeding);
                }
            }
        }

        // CRUD Methods for Food
        public void AddFood(Food food)
        {
            if (_foods != null)
            {
                food.Id = (_foods.Count > 0 ? _foods.Max(f => f.Id) : 0) + 1;
                _foods.Add(food);
            }
        }

        public void UpdateFood(Food food)
        {
            if (_foods != null)
            {
                var existing = _foods.FirstOrDefault(f => f.Id == food.Id);
                if (existing != null)
                {
                    existing.Name = food.Name;
                }
            }
        }

        public void DeleteFood(int foodId)
        {
            if (_foods != null)
            {
                var food = _foods.FirstOrDefault(f => f.Id == foodId);
                if (food != null)
                {
                    _foods.Remove(food);
                }
            }
        }

        public void SaveChanges()
        {
            // Mock repository doesn't persist data
        }
    }
}
