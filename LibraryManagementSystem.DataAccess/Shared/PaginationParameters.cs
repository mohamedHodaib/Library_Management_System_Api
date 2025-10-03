using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.Shared
{
    using System.ComponentModel.DataAnnotations;

    public class PaginationParameters
    {
        private const int MaxPageSize = 50;

        // Default value is 1. Attribute validates the input from the client.
        [Range(1, int.MaxValue, ErrorMessage = "Page number must be a positive integer.")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        // Attribute validates the input from the client.
        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 50.")]
        public int PageSize
        {
            get => _pageSize;
            // The setter acts as a final safeguard that caps the value.
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        // This is a great addition for flexible sorting!
        public string? OrderBy { get; set; }
    }

    public class SearchParameters : PaginationParameters
    {
        [Required]
        public string SearchTerm { get; set; } = null!;
    }

}
