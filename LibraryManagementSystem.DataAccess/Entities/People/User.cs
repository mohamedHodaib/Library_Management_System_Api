using LibraryManagementSystem.DataAccess.Contract;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.DataAccess.Entities.People
{
    public class User : IdentityUser
    {
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public bool IsRefreshTokenRevoked { get; set; } = false;
        public int PersonId { get; set; }
        public Person Person { get; set; } = null!;
    }
}
