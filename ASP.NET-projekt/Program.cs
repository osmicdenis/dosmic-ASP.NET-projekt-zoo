using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IZooRepository, MockZooRepository>();

var app = builder.Build();

// Get repository from dependency injection
var zooRepository = app.Services.GetRequiredService<IZooRepository>();

// LINQ Query Examples
// Get all carnivores in the zoo
var allAnimals = zooRepository.GetAllAnimals().ToList();
var carnivores = allAnimals.Where(a => a.Diet == DietType.Carnivore).ToList();
Console.WriteLine($"Total carnivores: {carnivores.Count()}");

// Get animals ordered by name
var animalsByName = allAnimals.OrderBy(a => a.Name).ToList();
Console.WriteLine($"First animal alphabetically: {animalsByName.First().Name}");

// Get animals in African Savanna enclosure
var enclosure1 = zooRepository.GetEnclosureById(1);
var savannahAnimals = enclosure1?.Animals.Where(a => a.Species.Contains("Lion")).ToList() ?? new List<Animal>();
Console.WriteLine($"Lions in Savanna: {savannahAnimals.Count()}");

// Get animals 8 years or older
var olderAnimals = allAnimals.Where(a => DateTime.Now.Year - a.DateOfBirth.Year >= 8).ToList();
Console.WriteLine($"Animals 8 years or older: {olderAnimals.Count()}");

// Get Koko and display medical records
var koko = allAnimals.FirstOrDefault(a => a.Name == "Koko");

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
