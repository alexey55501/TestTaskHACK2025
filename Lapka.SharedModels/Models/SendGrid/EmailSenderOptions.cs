namespace Lapka.SharedModels.Models
{
    public class EmailSenderOptions
    {
        public const string SectionName = "SendGrid";
        public string SendGridKey { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SimpleTemplateId { get; set; }
        public string TemplateWithActionId { get; set; }
    }
}

