using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Options
{
    public class ForgotPasswordSettings
    {
        public const string SectionName = "FrontendUrl";
        public string FrontendUrl { get; set; } = null!;
    }
}
