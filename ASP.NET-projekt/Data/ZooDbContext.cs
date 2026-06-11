using ASP.NET_projekt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.NET_projekt.Data
{
    public class ZooDbContext : IdentityDbContext<AppUser>
    {
        public ZooDbContext(DbContextOptions<ZooDbContext> options) : base(options)
        {
        }

        public DbSet<Zoo> Zoos { get; set; }
        public DbSet<Enclosure> Enclosures { get; set; }
        public DbSet<Animal> Animals { get; set; }
        public DbSet<Zookeeper> Zookeepers { get; set; }
        public DbSet<Veterinarian> Veterinarians { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Food> Foods { get; set; }
        public DbSet<Feeding> Feedings { get; set; }
        public DbSet<AnimalPhoto> AnimalPhotos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            
            // Zoo - Enclosure (1-to-many)
            modelBuilder.Entity<Enclosure>()
                .HasOne(e => e.Zoo)
                .WithMany(z => z.Enclosures)
                .HasForeignKey(e => e.ZooId)
                .OnDelete(DeleteBehavior.Cascade);

            // Zookeeper - Enclosure (1-to-many)
            modelBuilder.Entity<Enclosure>()
                .HasOne(e => e.Zookeeper)
                .WithMany(z => z.Enclosures)
                .HasForeignKey(e => e.ZookeeperId)
                .OnDelete(DeleteBehavior.Restrict);

            // Enclosure - Animal (1-to-many)
            modelBuilder.Entity<Animal>()
                .HasOne(a => a.Enclosure)
                .WithMany(e => e.Animals)
                .HasForeignKey(a => a.EnclosureId)
                .OnDelete(DeleteBehavior.SetNull);

            // Animal - MedicalRecord (1-to-many)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Animal)
                .WithMany(a => a.MedicalRecords)
                .HasForeignKey(m => m.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Veterinarian - MedicalRecord (1-to-many)
            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Veterinarian)
                .WithMany(v => v.MedicalRecords)
                .HasForeignKey(m => m.VeterinarianId)
                .OnDelete(DeleteBehavior.Restrict);

            // Animal - Feeding (1-to-many)
            modelBuilder.Entity<Feeding>()
                .HasOne(f => f.Animal)
                .WithMany(a => a.Feedings)
                .HasForeignKey(f => f.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Animal - Photo (1-to-many)
            modelBuilder.Entity<AnimalPhoto>()
                .HasOne(p => p.Animal)
                .WithMany(a => a.Photos)
                .HasForeignKey(p => p.AnimalId)
                .OnDelete(DeleteBehavior.Cascade);

            // Food - Feeding (1-to-many)
            modelBuilder.Entity<Feeding>()
                .HasOne(f => f.Food)
                .WithMany(fo => fo.Feedings)
                .HasForeignKey(f => f.FoodId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed Data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Zoos
            modelBuilder.Entity<Zoo>().HasData(
                new Zoo { Id = 1, Name = "Zagreb Zoo", Location = "Zagreb, Croatia" },
                new Zoo { Id = 2, Name = "Split Zoo", Location = "Split, Croatia" }
            );

            // Seed Zookeepers
            modelBuilder.Entity<Zookeeper>().HasData(
                new Zookeeper { Id = 1, FirstName = "Marko", LastName = "Horvat", YearsOfExperience = 8, DateOfEmployment = new DateTime(2016, 5, 15) },
                new Zookeeper { Id = 2, FirstName = "Ana", LastName = "Novak", YearsOfExperience = 5, DateOfEmployment = new DateTime(2019, 3, 20) },
                new Zookeeper { Id = 3, FirstName = "Ivan", LastName = "Petrović", YearsOfExperience = 12, DateOfEmployment = new DateTime(2012, 1, 10) }
            );

            // Seed Enclosures
            modelBuilder.Entity<Enclosure>().HasData(
                new Enclosure { Id = 1, ZooId = 1, Name = "African Savanna", Type = "Grassland", Capacity = 50, ZookeeperId = 1 },
                new Enclosure { Id = 2, ZooId = 1, Name = "Jungle Canopy", Type = "Tropical Forest", Capacity = 30, ZookeeperId = 2 },
                new Enclosure { Id = 3, ZooId = 1, Name = "Arctic Zone", Type = "Tundra", Capacity = 20, ZookeeperId = 3 },
                new Enclosure { Id = 4, ZooId = 2, Name = "Marine Aquarium", Type = "Aquatic", Capacity = 100, ZookeeperId = 1 }
            );

            // Seed Animals
            modelBuilder.Entity<Animal>().HasData(
                new Animal { Id = 1, Name = "Simba", Species = "Lion", Diet = DietType.Carnivore, DateOfBirth = new DateTime(2015, 6, 10), DateOfArrival = new DateTime(2017, 3, 15), EnclosureId = 1 },
                new Animal { Id = 2, Name = "Nala", Species = "Lioness", Diet = DietType.Carnivore, DateOfBirth = new DateTime(2016, 4, 5), DateOfArrival = new DateTime(2017, 3, 15), EnclosureId = 1 },
                new Animal { Id = 3, Name = "Koko", Species = "Gorilla", Diet = DietType.Herbivore, DateOfBirth = new DateTime(2012, 2, 20), DateOfArrival = new DateTime(2014, 5, 10), EnclosureId = 2 },
                new Animal { Id = 4, Name = "Elsa", Species = "Elephant", Diet = DietType.Herbivore, DateOfBirth = new DateTime(2008, 7, 15), DateOfArrival = new DateTime(2010, 1, 20), EnclosureId = 1 },
                new Animal { Id = 5, Name = "Nanook", Species = "Polar Bear", Diet = DietType.Carnivore, DateOfBirth = new DateTime(2014, 11, 3), DateOfArrival = new DateTime(2018, 2, 1), EnclosureId = 3 }
            );

            // Seed Veterinarians
            modelBuilder.Entity<Veterinarian>().HasData(
                new Veterinarian { Id = 1, FirstName = "Dr. Zdravko", LastName = "Medić" },
                new Veterinarian { Id = 2, FirstName = "Dr. Sonja", LastName = "Vidić" }
            );

            // Seed Medical Records
            modelBuilder.Entity<MedicalRecord>().HasData(
                new MedicalRecord { Id = 1, AnimalId = 1, VeterinarianId = 1, Diagnosis = "Regular Checkup", Therapy = "Vaccines", ExaminationDate = new DateTime(2024, 3, 15) },
                new MedicalRecord { Id = 2, AnimalId = 3, VeterinarianId = 2, Diagnosis = "Minor Injury on Left Paw", Therapy = "Bandaging and Rest", ExaminationDate = new DateTime(2024, 2, 20) },
                new MedicalRecord { Id = 3, AnimalId = 5, VeterinarianId = 1, Diagnosis = "Dental Cleaning", Therapy = "Professional Cleaning", ExaminationDate = new DateTime(2024, 1, 10) }
            );

            // Seed Foods
            modelBuilder.Entity<Food>().HasData(
                new Food { Id = 1, Name = "Meat" },
                new Food { Id = 2, Name = "Fruits" },
                new Food { Id = 3, Name = "Vegetables" },
                new Food { Id = 4, Name = "Fish" },
                new Food { Id = 5, Name = "Insects" }
            );

            // Seed Feedings
            modelBuilder.Entity<Feeding>().HasData(
                new Feeding { Id = 1, AnimalId = 1, FoodId = 1, FeedingTime = new DateTime(2024, 5, 4, 8, 0, 0) },
                new Feeding { Id = 2, AnimalId = 2, FoodId = 1, FeedingTime = new DateTime(2024, 5, 4, 8, 30, 0) },
                new Feeding { Id = 3, AnimalId = 3, FoodId = 2, FeedingTime = new DateTime(2024, 5, 4, 9, 0, 0) },
                new Feeding { Id = 4, AnimalId = 4, FoodId = 3, FeedingTime = new DateTime(2024, 5, 4, 9, 30, 0) },
                new Feeding { Id = 5, AnimalId = 5, FoodId = 1, FeedingTime = new DateTime(2024, 5, 4, 10, 0, 0) }
            );
        }
    }
}
