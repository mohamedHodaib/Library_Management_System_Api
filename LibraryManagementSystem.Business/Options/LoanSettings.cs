using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Options
{
    public class LoanSettings
    {
        public const string SectionName = "LoanSettings";
        public int DueDays {  get; set; }
        public int LoansLimit { get; set; }
    }
}
