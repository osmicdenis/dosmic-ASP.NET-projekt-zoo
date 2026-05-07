namespace ASP.NET_projekt.ViewModels
{
    public class StaffListItemViewModel
    {
        public int Id { get; set; }
        public required string Role { get; set; }
        public required string FullName { get; set; }
        public required string Subtitle { get; set; }
        public required string StatusText { get; set; }
        public required string StatusClass { get; set; }
        public required List<KeyValuePair<string, string>> MetaItems { get; set; }
    }
}
