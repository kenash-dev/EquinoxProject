using Microsoft.EntityFrameworkCore;

namespace Equinox.UI.Web.Models;

public class PaginatedList<T>: List<T>
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);

        this.AddRange(items);
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;

    //In microsoft documents the idea is to use the db-context model directly here and use EF core skip and take.
    //Since we have layered architecture, we are passing the count, pageIndex and pageSize to determine the behaviour of Previous and Next buttons.
    public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> items,int totalCustomerCount, int pageIndex, int pageSize)
    {
        return new PaginatedList<T>(items.ToList(), totalCustomerCount, pageIndex, pageSize);
    }
}