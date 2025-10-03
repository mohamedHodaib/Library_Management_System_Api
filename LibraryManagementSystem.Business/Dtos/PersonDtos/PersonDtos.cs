using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.PersonDtos
{

    public abstract class ManipulatePersonDto
    {
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 character.")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 character.")]
        public string LastName { get; set; } = null!;
    }


    public class CreatePersonDto : ManipulatePersonDto
    {
    }

    public class UpdatePersonDto : ManipulatePersonDto
    {
    }


    public class PersonSummaryDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Email { get; set; } //get it from related user if it has related user

    }

    public class PersonDetailsDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Role { get; set; } = null!;

    }
}
