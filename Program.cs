using ASP.NET_projekt.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();







// Create and initialize main objects in a branched manner
var zoo1 = new Zoo { Id = 1, Name = "Zoo Zagreb", Location = "Maksimir" };

var zookeeper1 = new Zookeeper { Id = 1, FirstName = "Denis", LastName = "Osmić", YearsOfExperience = 10, DateOfEmployment = new DateTime(2015, 4, 1) };
var zookeeper2 = new Zookeeper { Id = 2, FirstName = "Danijel", LastName = "Osmić", YearsOfExperience = 5, DateOfEmployment = new DateTime(2018, 9, 15) };

var veterinarian1 = new Veterinarian { Id = 1, FirstName = "Ivan", LastName = "Horvat" };

var enclosure1 = new Enclosure
{
    Id = 1,
    Zoo = zoo1,
    Name = "African Savanna",
    Type = "Open Grassland",
    Capacity = 5,
    Zookeeper = zookeeper1,
    Animals = new List<Animal>
    {
        new Animal { Id = 1, Name = "Simba", Species = "Lion", DateOfBirth = new DateTime(2019, 5, 12), DateOfArrival = new DateTime(2020, 2, 20), Diet = DietType.Carnivore, MedicalRecords = new List<MedicalRecord>{} },
        new Animal { Id = 2, Name = "Nala", Species = "Lion", DateOfBirth = new DateTime(2020, 8, 22), DateOfArrival = new DateTime(2021, 3, 10), Diet = DietType.Carnivore }
    }
};

var enclosure2 = new Enclosure 
{ 
    Id = 2,
    Zoo = zoo1,
    Name = "Tropical Forest", 
    Type = "Dense Forest", 
    Capacity = 8,
    Zookeeper = zookeeper2,
    Animals = new List<Animal>
    {
        new Animal { Id = 3, Name = "Koko", Species = "Gorilla", DateOfBirth = new DateTime(2018, 11, 3), DateOfArrival = new DateTime(2020, 6, 5), Diet = DietType.Omnivore, MedicalRecords = new List<MedicalRecord>{} }
    }
};

var enclosure3 = new Enclosure
{
    Id = 3,
    Zoo = zoo1,
    Name = "Reptile House",
    Type = "Indoor",
    Capacity = 10,
    Zookeeper = zookeeper1,
    Animals = new List<Animal>
    {
        new Animal { Id = 4, Name = "Rex", Species = "Crocodile", DateOfBirth = new DateTime(2010, 4, 18), DateOfArrival = new DateTime(2012, 9, 30), Diet = DietType.Carnivore, MedicalRecords = new List<MedicalRecord>{} }
    }
};


// Create medical record for Koko
var kokoMedicalRecord = new MedicalRecord 
{ 
    Id = 1, 
    Diagnosis = "Mild respiratory infection", 
    Therapy = "Antibiotics and rest", 
    ExaminationDate = new DateTime(2025, 3, 15, 10, 30, 0), 
    Veterinarian = veterinarian1, 
    Animal = enclosure2.Animals.First(a => a.Name == "Koko") 
};
enclosure2.Animals.First(a => a.Name == "Koko").MedicalRecords.Add(kokoMedicalRecord);


// LINQ Query Examples
// Get all carnivores in the zoo
var allEnclosures = new List<Enclosure> { enclosure1, enclosure2, enclosure3 };
var carnivores = allEnclosures.SelectMany(e => e.Animals).Where(a => a.Diet == DietType.Carnivore).ToList();
Console.WriteLine($"Total carnivores: {carnivores.Count()}");

// Get animals ordered by name
var animalsByName = allEnclosures.SelectMany(e => e.Animals).OrderBy(a => a.Name).ToList();
Console.WriteLine($"First animal alphabetically: {animalsByName.First().Name}");

// Get animals in African Savanna enclosure
var savannahAnimals = enclosure1.Animals.Where(a => a.Species.Contains("Lion")).ToList();
Console.WriteLine($"Lions in Savanna: {savannahAnimals.Count()}");


var olderAnimals = allEnclosures.SelectMany(e => e.Animals).Where(a => DateTime.Now.Year - a.DateOfBirth.Year >= 8).ToList();
Console.WriteLine($"Animals 8 years or older: {olderAnimals.Count()}");

var koko = allEnclosures.SelectMany(e => e.Animals).FirstOrDefault(a => a.Name == "Koko");

if (koko == null)
{
    Console.WriteLine("Koko not found.");
}
else if (koko.MedicalRecords.Count == 0)
{
    Console.WriteLine("No medical records for Koko.");
}
else
{
    Console.WriteLine($"Medical Records for {koko.Name}:");
    foreach (var record in koko.MedicalRecords)
    {
        Console.WriteLine($"  Id: {record.Id}");
        Console.WriteLine($"  Diagnosis: {record.Diagnosis}");
        Console.WriteLine($"  Therapy: {record.Therapy}");
        Console.WriteLine($"  Date: {record.ExaminationDate:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
