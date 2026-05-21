using ASP.NET_projekt.Models;
using ASP.NET_projekt.Repositories;
using ASP.NET_projekt.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure Entity Framework DbContext
builder.Services.AddDbContext<ZooDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("ZooDbContext"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.MigrationsAssembly("ASP.NET-projekt");
        }));

// Register EF Repository (replaces MockRepository)
builder.Services.AddScoped<IZooRepository, EfZooRepository>();

var app = builder.Build();

// Note: Repository test code commented out - will be replaced with EF Repository
/*
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
*/

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "animals-list",
    pattern: "animals",
    defaults: new { controller = "Animals", action = "Index" });

app.MapControllerRoute(
    name: "animal-details",
    pattern: "animals/{id:int}",
    defaults: new { controller = "Animals", action = "Details" });

app.MapControllerRoute(
    name: "enclosures-list",
    pattern: "enclosures",
    defaults: new { controller = "Enclosures", action = "Index" });

app.MapControllerRoute(
    name: "enclosure-details",
    pattern: "enclosures/{id:int}",
    defaults: new { controller = "Enclosures", action = "Details" });

app.MapControllerRoute(
    name: "feeding-schedule",
    pattern: "feeding-schedule",
    defaults: new { controller = "Feedings", action = "Index" });

app.MapControllerRoute(
    name: "staff-list",
    pattern: "staff",
    defaults: new { controller = "Staff", action = "Index" });

app.MapControllerRoute(
    name: "staff-details",
    pattern: "staff/{role:regex(^(zookeeper|veterinarian)$)}/{id:int}",
    defaults: new { controller = "Staff", action = "Details" });

app.MapControllerRoute(
    name: "zoo-map",
    pattern: "zoo-map",
    defaults: new { controller = "ZooMap", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Open browser automatically when app starts
_ = Task.Run(async () =>
{
    await Task.Delay(1000); // Wait for app to be ready
    if (app.Urls.Any())
    {
        var url = app.Urls.First();
        try
        {
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
        }
        catch
        {
            // Silently fail if browser launch doesn't work
        }
    }
});

app.Run();
