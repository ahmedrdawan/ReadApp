using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyReadsApp.Core.AppSetting
{
    public class StmpSetting
    {
        public string SenderEmail { get; set; }
        public string SmtpPassword { get; set; } 
        public string SmtpHost { get; set; }     
        public int SmtpPort { get; set; }       
        public bool EnableSsl { get; set; }     
    }
}
