using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ASP.NET_projekt.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Foods",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Meat" },
                    { 2, "Fruits" },
                    { 3, "Vegetables" },
                    { 4, "Fish" },
                    { 5, "Insects" }
                });

            migrationBuilder.InsertData(
                table: "Veterinarians",
                columns: new[] { "Id", "FirstName", "LastName" },
                values: new object[,]
                {
                    { 1, "Dr. Zdravko", "Medić" },
                    { 2, "Dr. Sonja", "Vidić" }
                });

            migrationBuilder.InsertData(
                table: "Zookeepers",
                columns: new[] { "Id", "DateOfEmployment", "FirstName", "LastName", "YearsOfExperience" },
                values: new object[,]
                {
                    { 1, new DateTime(2016, 5, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Marko", "Horvat", 8 },
                    { 2, new DateTime(2019, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ana", "Novak", 5 },
                    { 3, new DateTime(2012, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ivan", "Petrović", 12 }
                });

            migrationBuilder.InsertData(
                table: "Zoos",
                columns: new[] { "Id", "Location", "Name" },
                values: new object[,]
                {
                    { 1, "Zagreb, Croatia", "Zagreb Zoo" },
                    { 2, "Split, Croatia", "Split Zoo" }
                });

            migrationBuilder.InsertData(
                table: "Enclosures",
                columns: new[] { "Id", "Capacity", "Name", "Type", "ZooId", "ZookeeperId" },
                values: new object[,]
                {
                    { 1, 50, "African Savanna", "Grassland", 1, 1 },
                    { 2, 30, "Jungle Canopy", "Tropical Forest", 1, 2 },
                    { 3, 20, "Arctic Zone", "Tundra", 1, 3 },
                    { 4, 100, "Marine Aquarium", "Aquatic", 2, 1 }
                });

            migrationBuilder.InsertData(
                table: "Animals",
                columns: new[] { "Id", "DateOfArrival", "DateOfBirth", "Diet", "EnclosureId", "Name", "Species" },
                values: new object[,]
                {
                    { 1, new DateTime(2017, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2015, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Simba", "Lion" },
                    { 2, new DateTime(2017, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2016, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, "Nala", "Lioness" },
                    { 3, new DateTime(2014, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2012, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 2, "Koko", "Gorilla" },
                    { 4, new DateTime(2010, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2008, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0, 1, "Elsa", "Elephant" },
                    { 5, new DateTime(2018, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2014, 11, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 3, "Nanook", "Polar Bear" }
                });

            migrationBuilder.InsertData(
                table: "Feedings",
                columns: new[] { "Id", "AnimalId", "FeedingTime", "FoodId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 5, 4, 8, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, new DateTime(2024, 5, 4, 8, 30, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 3, 3, new DateTime(2024, 5, 4, 9, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 4, 4, new DateTime(2024, 5, 4, 9, 30, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 5, 5, new DateTime(2024, 5, 4, 10, 0, 0, 0, DateTimeKind.Unspecified), 1 }
                });

            migrationBuilder.InsertData(
                table: "MedicalRecords",
                columns: new[] { "Id", "AnimalId", "Diagnosis", "ExaminationDate", "Therapy", "VeterinarianId" },
                values: new object[,]
                {
                    { 1, 1, "Regular Checkup", new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Vaccines", 1 },
                    { 2, 3, "Minor Injury on Left Paw", new DateTime(2024, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Bandaging and Rest", 2 },
                    { 3, 5, "Dental Cleaning", new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Professional Cleaning", 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Feedings",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Feedings",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Feedings",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Feedings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Feedings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MedicalRecords",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Animals",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Foods",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Veterinarians",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Veterinarians",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Zoos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Enclosures",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Zookeepers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Zookeepers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Zookeepers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Zoos",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
