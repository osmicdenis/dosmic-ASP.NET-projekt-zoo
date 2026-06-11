using System.ComponentModel.DataAnnotations;

namespace ASP.NET_projekt.Models
{
    public class AnimalPhoto
    {
        [Key]
        public int Id { get; set; }

        public int AnimalId { get; set; }
        public virtual Animal? Animal { get; set; }

        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public required string ContentType { get; set; }
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}