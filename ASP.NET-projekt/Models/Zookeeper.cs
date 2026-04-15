namespace ASP.NET_projekt.Models
{
    public class Zookeeper
    {
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public int YearsOfExperience { get; set; }
    public DateTime DateOfEmployment { get; set; }

    public List<Enclosure> Enclosures { get; set; } = new List<Enclosure>();
    }
}