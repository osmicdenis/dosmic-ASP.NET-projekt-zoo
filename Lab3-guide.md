# Lab 3 Study Guide for This Project

This document explains the Entity Framework, routing, repository, and view-model parts of the zoo project using the actual code in the workspace. It is meant to help you understand what each class/file does and how the pieces fit together for Lab 3.

## 1. The Big Picture

The project is an ASP.NET MVC app with these main layers:

- **Models** represent database entities such as animals, enclosures, zoos, staff, and feedings.
- **DbContext** connects the model classes to SQL Server through Entity Framework Core.
- **Repository** wraps database access so controllers do not talk to EF directly everywhere.
- **ViewModels** package data for Razor views, especially forms and list pages.
- **Migrations** describe how EF creates and updates the database schema.
- **Routing** maps friendly URLs like `/animals` or `/feeding-schedule` to controller actions.

If you understand those five pieces, you understand most of Lab 3.

## 2. EF Model Classes: What They Represent

The classes in [ASP.NET-projekt/Models](ASP.NET-projekt/Models) are the EF entity classes. Each class typically becomes a database table.

### Zoo
File: [Zoo.cs](ASP.NET-projekt/Models/Zoo.cs)

This class represents a zoo.

- `Id` is the primary key.
- `Name` and `Location` are the zoo’s basic fields.
- `Enclosures` is a navigation collection.

That collection means one zoo can have many enclosures.

### Enclosure
File: [Enclosure.cs](ASP.NET-projekt/Models/Enclosure.cs)

This class represents an enclosure inside a zoo.

- `ZooId` is a foreign key to the zoo it belongs to.
- `Zoo` is the navigation property to that zoo.
- `ZookeeperId` is a foreign key to the assigned zookeeper.
- `Zookeeper` is the navigation property to that staff member.
- `Animals` is the collection of animals living in the enclosure.

This is a good example of a 1-to-many relationship:

- one zoo -> many enclosures
- one zookeeper -> many enclosures
- one enclosure -> many animals

### Animal
File: [Animal.cs](ASP.NET-projekt/Models/Animal.cs)

This is one of the most important classes in the project.

- `Id` is the primary key.
- `Name`, `Species`, `DateOfBirth`, `DateOfArrival`, and `Diet` are the animal fields.
- `EnclosureId` links the animal to an enclosure.
- `Enclosure` is the related enclosure.
- `MedicalRecords` is the list of health records.
- `Feedings` is the list of feeding records.

In the app, the animal details page and edit page both rely heavily on this model.

### Zookeeper
File: [Zookeeper.cs](ASP.NET-projekt/Models/Zookeeper.cs)

This represents a zookeeper.

- `Id` is the primary key.
- `FirstName`, `LastName`, `YearsOfExperience`, and `DateOfEmployment` store staff data.
- `Enclosures` shows which enclosures this zookeeper is assigned to.

### Veterinarian
File: [Veterinarian.cs](ASP.NET-projekt/Models/Veterinarian.cs)

This represents a veterinarian.

- `Id`, `FirstName`, and `LastName` describe the vet.
- `MedicalRecords` is the list of animal records handled by that vet.

### MedicalRecord
File: [MedicalRecord.cs](ASP.NET-projekt/Models/MedicalRecord.cs)

This class stores animal health information.

- `Diagnosis`, `Therapy`, and `ExaminationDate` are the actual medical details.
- `AnimalId` links the record to an animal.
- `VeterinarianId` links the record to a veterinarian.

This gives you two foreign keys in one entity.

### Food
File: [Food.cs](ASP.NET-projekt/Models/Food.cs)

This is a lookup-style entity for food types.

- `Id` and `Name` identify the food.
- `Feedings` shows which feeding records used that food.

### Feeding
File: [Feeding.cs](ASP.NET-projekt/Models/Feeding.cs)

This connects an animal to food at a time.

- `AnimalId` links to the animal.
- `FoodId` links to the food.
- `FeedingTime` stores the time of feeding.

This is a good example of a relationship entity.

### DietType
File: [DietType.cs](ASP.NET-projekt/Models/DietType.cs)

This is an enum, not a table.

It is used by `Animal.Diet` to represent herbivore, carnivore, or omnivore.

## 3. How EF Is Configured

### ZooDbContext
File: [ZooDbContext.cs](ASP.NET-projekt/Data/ZooDbContext.cs)

This is the core EF class.

It does three jobs:

1. It declares the tables through `DbSet<T>` properties:
   - `Zoos`
   - `Enclosures`
   - `Animals`
   - `Zookeepers`
   - `Veterinarians`
   - `MedicalRecords`
   - `Foods`
   - `Feedings`

2. It configures relationships in `OnModelCreating`.

3. It seeds initial data with `HasData(...)`.

### Relationship examples in ZooDbContext

These lines are the EF rules that matter most:

- `Enclosure` has one `Zoo`, and `Zoo` has many `Enclosures`.
- `Enclosure` has one `Zookeeper`, and `Zookeeper` has many `Enclosures`.
- `Animal` has one `Enclosure`, and `Enclosure` has many `Animals`.
- `MedicalRecord` belongs to one `Animal` and one `Veterinarian`.
- `Feeding` belongs to one `Animal` and one `Food`.

The `OnDelete(...)` calls define what happens if a related record is deleted.

For example:

- `Cascade` means child rows are removed too.
- `Restrict` means deletion is blocked if related rows exist.
- `SetNull` means the foreign key is cleared.

### Why this matters

If someone asks what `ZooDbContext` is, the short answer is:

> It is the EF bridge between the C# model classes and the SQL database.

## 4. Migrations: How the Database Is Created and Updated

Migrations are EF’s way of converting model changes into database changes.

### Initial migration
File: [20260504100249_Initial.cs](ASP.NET-projekt/Migrations/20260504100249_Initial.cs)

This migration creates the first version of the database schema.

It creates tables such as:

- `Foods`
- `Veterinarians`
- `Zookeepers`
- `Zoos`
- `Enclosures`
- `Animals`
- `Feedings`
- `MedicalRecords`

It also defines foreign keys and indexes.

### Snapshot file
File: [ZooDbContextModelSnapshot.cs](ASP.NET-projekt/Migrations/ZooDbContextModelSnapshot.cs)

This is EF’s saved picture of the model at the last migration.

It is not something you edit manually. EF uses it to compare:

- the current model in `ZooDbContext`
- the last known model in the snapshot

That comparison tells EF what new migration to generate.

### Foreign key fix migration
File: [20260504100535_FixZookeeperForeignKey.cs](ASP.NET-projekt/Migrations/20260504100535_FixZookeeperForeignKey.cs)

This migration fixes a mistake from the first migration.

Originally, EF created `ZookeeperIdId` in `Enclosures`. This migration renames it to `ZookeeperId` and updates the foreign key and index.

This is a useful example to mention orally: migrations are not only for new tables, but also for fixing schema mistakes.

### Seed data migration
File: [20260504101931_SeedData.cs](ASP.NET-projekt/Migrations/20260504101931_SeedData.cs)

This migration inserts initial data into the tables.

It adds:

- zoos
- zookeepers
- enclosures
- animals
- veterinarians
- medical records
- foods
- feedings

This is why the project already has sample data when the database is created.

### Important migration idea

The migration files do not mirror every new row you add in the running app.

If you add a new animal through the UI, that animal goes into the database, not automatically into `ZooDbContext.cs` or the migration snapshot.

## 5. Repository: Where Database Queries Are Kept

### EfZooRepository
File: [EfZooRepository.cs](ASP.NET-projekt/Repositories/EfZooRepository.cs)

This class wraps the DbContext and gives the controllers a clean data-access layer.

Instead of controllers directly writing EF queries everywhere, they call repository methods like:

- `GetAllAnimals()`
- `GetAnimalById(id)`
- `GetAllEnclosures()`
- `GetZooById(id)`
- `AddAnimal(animal)`
- `UpdateAnimal(animal)`
- `SaveChanges()`

### What it does well

It centralizes database access.

That means:

- less duplicated query code
- controllers stay simpler
- data loading logic is grouped in one place

### Example of eager loading

Many repository methods use `Include` and `ThenInclude`.

Example idea from `GetAnimalById()`:

- load the animal
- load the enclosure
- load the medical records
- load each record’s veterinarian
- load feedings and each feeding’s food

This is important because the detail pages need related data immediately.

### Simple way to explain the repository

> The repository is the app’s database access service. Controllers ask it for data, and it asks EF for the database.

## 6. ViewModel for Animal Forms

### AnimalCreateEditViewModel
File: [AnimalCreateEditViewModel.cs](ASP.NET-projekt/ViewModels/AnimalCreateEditViewModel.cs)

This class is used by the Create/Edit animal form.

It is not the same as the `Animal` entity.

It contains:

- the animal fields that the form edits
- validation attributes
- the optional enclosure selection
- the list of available enclosures for the dropdown

### Why this exists

A database entity is not always the best shape for a form.

The view model can combine:

- values to show
- validation rules
- dropdown data

That is why the edit page can display a list of enclosures even though the animal entity alone cannot supply that list.

### EnclosureOption helper

`EnclosureOption` is a tiny helper class used to build the dropdown list.

It stores only:

- `Id`
- `Name`

## 7. The Animal Edit View

File: [Views/Animals/Edit.cshtml](ASP.NET-projekt/Views/Animals/Edit.cshtml)

This Razor view renders the edit form.

### The enclosure dropdown

This part is especially important:

```csharp
<select asp-for="EnclosureId" asp-items="@(new SelectList(Model.AvailableEnclosures, "Id", "Name"))" class="form-select">
    <option value="">-- Select Enclosure (Optional) --</option>
</select>
```

It means:

- `asp-for="EnclosureId"` binds the selected value back to the view model
- `asp-items` fills the dropdown from `AvailableEnclosures`
- the optional blank item allows no enclosure assignment

### How to explain it simply

> The controller gives the view a list of enclosures, and the view turns that list into a dropdown so the user can choose where the animal belongs.

## 8. Routing: Friendly URLs

Routing is configured in [Program.cs](ASP.NET-projekt/Program.cs).

The app uses custom route patterns so the URLs are nicer than the default `/Controller/Action` style.

Examples from the project:

- `/animals` -> `AnimalsController.Index`
- `/animals/{id}` -> `AnimalsController.Details`
- `/enclosures/{id}` -> `EnclosuresController.Details`
- `/feeding-schedule` -> `FeedingsController.Index`
- `/staff/{role}/{id}` -> `StaffController.Details`
- `/zoo-map` -> `ZooMapController.Index`

### Why this matters for Lab 3

The lab requires custom routing for at least four actions.

Your project already goes beyond that.

### Simple oral-exam explanation

> Routing is the part of ASP.NET MVC that connects a URL to the controller action that should handle it.

## 9. Zoo Map Script

File: [Views/ZooMap/Index.cshtml](ASP.NET-projekt/Views/ZooMap/Index.cshtml)

The Razor view renders the zoo map and the enclosure hotspots.

The JavaScript section at the bottom makes the page interactive.

### What the script does

- finds the map container
- finds the info panel fields
- finds all hotspot buttons
- updates the panel when a hotspot is hovered, focused, or clicked
- marks the active hotspot visually
- on mobile, scrolls the panel into view

### Why it is useful

It shows how the page can update without a full page reload.

The Razor code sends data to the page through `data-*` attributes, and JavaScript reads those values and updates the panel.

## 10. How the Pieces Fit Together

Here is the full flow in your project:

1. `ZooDbContext` defines the EF model and relationships.
2. Migrations turn that model into tables and seed data.
3. `EfZooRepository` queries the database with EF.
4. Controllers ask the repository for data.
5. ViewModels package the data for views.
6. Razor views display the data and forms.
7. Routing connects friendly URLs to the right controller actions.

That is the whole EF/MVC pipeline for this lab.

## 11. What You Should Be Able to Say in Class

If the professor asks you to explain the EF part, a good short answer is:

> The project uses Entity Framework Core with `ZooDbContext` as the database context. The entity classes in `Models` map to tables, relationships are configured with foreign keys and navigation properties, migrations create and update the database, and `EfZooRepository` is used to load and save data through EF instead of mock objects.

If they ask about routing:

> The app defines custom routes in `Program.cs` so the URLs are cleaner, such as `/animals`, `/feeding-schedule`, and `/zoo-map`, and each route maps to a specific controller action.

If they ask about the edit form:

> The edit page uses `AnimalCreateEditViewModel` to carry the animal data plus the enclosure list for the dropdown.

## 12. Quick File List

Most relevant files for Lab 3:

- [ASP.NET-projekt/Data/ZooDbContext.cs](ASP.NET-projekt/Data/ZooDbContext.cs)
- [ASP.NET-projekt/Repositories/EfZooRepository.cs](ASP.NET-projekt/Repositories/EfZooRepository.cs)
- [ASP.NET-projekt/ViewModels/AnimalCreateEditViewModel.cs](ASP.NET-projekt/ViewModels/AnimalCreateEditViewModel.cs)
- [ASP.NET-projekt/Views/Animals/Edit.cshtml](ASP.NET-projekt/Views/Animals/Edit.cshtml)
- [ASP.NET-projekt/Views/ZooMap/Index.cshtml](ASP.NET-projekt/Views/ZooMap/Index.cshtml)
- [ASP.NET-projekt/Program.cs](ASP.NET-projekt/Program.cs)
- [ASP.NET-projekt/appsettings.json](ASP.NET-projekt/appsettings.json)
- [ASP.NET-projekt/Migrations/20260504100249_Initial.cs](ASP.NET-projekt/Migrations/20260504100249_Initial.cs)
- [ASP.NET-projekt/Migrations/20260504100535_FixZookeeperForeignKey.cs](ASP.NET-projekt/Migrations/20260504100535_FixZookeeperForeignKey.cs)
- [ASP.NET-projekt/Migrations/20260504101931_SeedData.cs](ASP.NET-projekt/Migrations/20260504101931_SeedData.cs)
- [ASP.NET-projekt/Migrations/ZooDbContextModelSnapshot.cs](ASP.NET-projekt/Migrations/ZooDbContextModelSnapshot.cs)

## 13. Final Summary

For Lab 3, the most important idea is that the project is no longer using a mock repository for data. It now uses Entity Framework Core, a real `DbContext`, migrations, seeded data, and custom routing. The zoo domain classes are mapped to tables, the repository hides the EF queries from controllers, and the views use view models to display and edit data cleanly.

## 14. Short Oral-Exam Version

If you need a very short answer in class, use these lines:

- **What is `ZooDbContext`?** It is the EF Core context that connects the model classes to the SQL database.
- **What are the model classes?** They are the entity classes in `Models`; each one maps to a table.
- **What do `Id`, `ForeignKey`, `virtual`, and `ICollection<T>` mean?** `Id` is the primary key, `ForeignKey` marks the FK column, `virtual` enables navigation properties, and `ICollection<T>` is used for 1-to-many collections.
- **What are migrations?** They are EF files that create and update the database schema from the model.
- **What is the snapshot file?** It is EF’s saved copy of the model state used to generate the next migration.
- **What is `EfZooRepository`?** It is the database access layer that wraps EF queries so controllers stay simpler.
- **What is `AnimalCreateEditViewModel`?** It is a form model that carries animal data plus dropdown data for the edit/create page.
- **What does routing do?** It maps friendly URLs like `/animals` or `/zoo-map` to controller actions.
- **What does the zoo map script do?** It updates the info panel when a hotspot is hovered, focused, or clicked.

### One-sentence summary

The project uses EF Core to store zoo data in a real SQL database, uses a repository to read and write that data, uses view models for forms, and uses custom routing so the app has cleaner URLs.
