namespace MyReadsApp.Core.AppSetting
{
    public class SmtpSettings
    {
        public string SenderEmail { get; set; }
        public string SmtpPassword { get; set; } 
        public string SmtpHost { get; set; }     
        public int SmtpPort { get; set; }       
        public bool EnableSsl { get; set; }     
    }
}
