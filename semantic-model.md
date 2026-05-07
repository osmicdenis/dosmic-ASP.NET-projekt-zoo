# Semantic Model

This project uses an Entity Framework Core model for a zoo management system. The model is made of classes that map to database tables, with navigation properties and foreign keys describing how the tables relate to each other.

## Main Models / Tables

- **Zoo**: `Id`, `Name`, `Location`
- **Enclosure**: `Id`, `ZooId`, `ZookeeperId`, `Name`, `Type`, `Capacity`
- **Animal**: `Id`, `EnclosureId`, `Name`, `Species`, `DateOfBirth`, `DateOfArrival`, `Diet`
- **Zookeeper**: `Id`, `FirstName`, `LastName`, `YearsOfExperience`, `DateOfEmployment`
- **Veterinarian**: `Id`, `FirstName`, `LastName`
- **MedicalRecord**: `Id`, `AnimalId`, `VeterinarianId`, `Diagnosis`, `Therapy`, `ExaminationDate`
- **Food**: `Id`, `Name`
- **Feeding**: `Id`, `AnimalId`, `FoodId`, `FeedingTime`
- **DietType**: enum used by `Animal.Diet` with values `Herbivore`, `Carnivore`, `Omnivore`

## Relationships

- **Zoo 1 → many Enclosure**
- **Zookeeper 1 → many Enclosure**
- **Enclosure 1 → many Animal**
- **Animal 1 → many MedicalRecord**
- **Veterinarian 1 → many MedicalRecord**
- **Animal 1 → many Feeding**
- **Food 1 → many Feeding**

## What It Does

This semantic model documents the database structure in human-readable form. It shows which entities exist, which properties store their data, and how the entities are connected. That makes the database easier to understand, maintain, and extend because the core domain rules are visible in one place.