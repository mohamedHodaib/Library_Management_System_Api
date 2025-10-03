using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Contract
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail,string resetLink);
    }
}
