using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Business.Dtos.BorrowerDtos
{

    public class CreateBorrowerDto
    {
        [Range(1, int.MaxValue)]
        public int PersonId { get; set; }
    }

    public class BorrowerDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public int CurrentLoansCount { get; set; }
    }


    public class BorrowerSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }


   
}
