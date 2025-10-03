using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Options
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";
        public string FromEmail { get; set; } = null!;
        public string SmtpServer { get; set; } = null!;
        public int    SmtpPort { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
