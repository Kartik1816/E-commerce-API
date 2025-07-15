using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.ViewModels;

namespace UserManagement.Domain.utils;

public class PaginationService
{
    public async Task<PaginatedResponse<T>> GetPaginatedDataAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        int totalRecords = await query.CountAsync();
        List<T> data = await query.Skip((pageNumber - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

        return new PaginatedResponse<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
            HasNextPage = pageNumber < (int)Math.Ceiling((double)totalRecords / pageSize),
            HasPreviousPage = pageNumber > 1,
            TotalDataOfPage = data.Count
        };
    }
}
