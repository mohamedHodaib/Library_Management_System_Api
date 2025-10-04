using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Options
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string? Key{ get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
        public double ExpireMinutes { get; set; }
        public double RefreshTokenExpireDays { get; set; }
    }
}
