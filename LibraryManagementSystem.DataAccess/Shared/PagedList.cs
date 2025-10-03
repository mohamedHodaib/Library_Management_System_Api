using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.DataAccess.Shared
{
    public class MetaData
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }


    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; }

        public PagedList(List<T> items,int count,int pageNumber,int pageSize)
        {
            MetaData = new MetaData
            {
                TotalCount = count,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            };

            AddRange(items);
        }


        public async static Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            int count = await source.CountAsync();

            var items = await source
                        .Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
