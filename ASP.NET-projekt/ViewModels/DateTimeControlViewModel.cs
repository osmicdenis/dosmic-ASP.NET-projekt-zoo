namespace ASP.NET_projekt.ViewModels
{
    public class DateTimeControlViewModel
    {
        public string FieldName { get; set; } = string.Empty;
        public string LabelText { get; set; } = string.Empty;
        public string Placeholder { get; set; } = string.Empty;
        public DateTime? SelectedValue { get; set; }
        public bool IsRequired { get; set; }
        public string? HelpText { get; set; }
        public string Format { get; set; } = "Y-m-d H:i";
        public bool EnableTime { get; set; } = true;
        public string TimeFormat { get; set; } = "H:i";
    }
}