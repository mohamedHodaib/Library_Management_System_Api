using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.UserDtos
{
    public class UserProfileDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class UserSummaryDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!; // From the related Person entity
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }

    public class CreateUserByAdminDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
    
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set;}

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; } = null!;
    
        [Required]
        [AllowedValues("Admin","Author","Borrower")]
        public string Role {get; set;} = null!; // e.g., "Admin", "Author", "Borrower"
    }


    public class UpdateUserProfileDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? Phone { get; set; }

    }


    public class AssignRoleDto
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [AllowedValues("Admin", "Author", "Borrower"
            ,ErrorMessage = "Role Field must be from the following values Admin,Author,Borrower")]
        public string RoleName { get; set; } = null!;
    }



}
