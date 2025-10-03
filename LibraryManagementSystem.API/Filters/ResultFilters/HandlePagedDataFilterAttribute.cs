using LibraryManagementSystem.Business.Dtos.Shared;
using LibraryManagementSystem.DataAccess.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;

namespace LibraryManagementSystem.API.Filters.ResultFilters
{
    public class HandlePagedDataFilterAttribute : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Result is ObjectResult objectResult 
                && objectResult.Value is IHasMetaData pagedListDto)
            {
                var paginationHeader = JsonSerializer.Serialize(pagedListDto.MetaData, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase

                });

                context.HttpContext.Response.Headers.Append("X-Pagination", paginationHeader);
            }

            await next();
        }
    }
}
