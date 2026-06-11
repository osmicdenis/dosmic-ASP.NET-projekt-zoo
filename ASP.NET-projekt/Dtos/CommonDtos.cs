namespace ASP.NET_projekt.Dtos
{
    public class LookupDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }

    public class EnclosureSummaryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Type { get; set; }
    }

    public class AnimalSummaryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Species { get; set; }
    }

    public class FoodSummaryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }
    }

    public class ZooSummaryDto
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public required string Location { get; set; }
    }

    public class ZookeeperSummaryDto
    {
        public int Id { get; set; }

        public required string FullName { get; set; }
    }

    public class VeterinarianSummaryDto
    {
        public int Id { get; set; }

        public required string FullName { get; set; }
    }
}