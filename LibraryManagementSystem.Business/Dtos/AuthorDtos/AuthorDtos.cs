using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.AuthorDtos
{
   public class CreateAuthorDto
   {
       [Required]
       [StringLength(200, MinimumLength = 8, ErrorMessage = "Biography must be from 8 to 200 characters.")]
       public string Biography { get; set; } = null!;

       [Required]
       [Range(1,int.MaxValue)]
       public int PersonId { get; set;}
   }

   public class UpdateAuthorDto
   {
        [Required]
        [StringLength(200, MinimumLength = 8, ErrorMessage = "Biography must be from 8 to 200 characters.")]
        public string Biography { get; set; } = null!;
   }


    public class AuthorSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }


    public class AuthorDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Biography { get; set; } = null!;
        public int BooksCount { get; set; }
    }


    public class AuthorStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int TotalBooksWritten { get; set; }
        public int TotalTimesBorrowed { get; set; } // Sum of borrows for all their books
        public string? MostPopularBookTitle { get; set; } 
    }


    
}
