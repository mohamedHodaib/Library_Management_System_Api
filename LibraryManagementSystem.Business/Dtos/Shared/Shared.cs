using LibraryManagementSystem.DataAccess.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Business.Dtos.Shared
{
    public class GetByIdsDto
    {
        [Required]
        [MinLength(1)]
        public IEnumerable<int> Ids { get; set; } = null!;
    }

    public class CreateCollectionDto<T> where T : class
    {
        [Required]
        [MinLength(1)]
        public IEnumerable<T>CreateDtos  { get; set; } = null!;
    }

    public interface IHasMetaData
    {
        MetaData MetaData { get; set; }
    }

    public class PagedListDto<T> : IHasMetaData
    {
        public IEnumerable<T> Items { get; set; } = null!;
        public MetaData MetaData { get; set; } = null!;
    }

}
