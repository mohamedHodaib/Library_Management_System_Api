using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.BorrowingDtos
{
   public class BorrowingDetailsDto
   {
        public int Id { get; set; }
        public string BookTitle { get; set; } = null!;
        public string BorrowerName { get; set; } = null!;
        public DateOnly BorrowingDate { get; set; }
        public DateOnly DueDate { get; set; }


   }
}
