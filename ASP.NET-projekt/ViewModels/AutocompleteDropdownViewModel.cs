namespace ASP.NET_projekt.ViewModels
{
    public class AutocompleteDropdownViewModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string LabelText { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public string SearchUrl { get; set; } = string.Empty;
        public string? SelectedText { get; set; }
        public string? SelectedValue { get; set; }
        public string ValueField { get; set; } = "id";
        public string LabelField { get; set; } = "name";
        public string QueryParam { get; set; } = "query";
        public int MinCharacters { get; set; } = 1;
        public bool IsRequired { get; set; }
        public string? HelpText { get; set; }
    }
}