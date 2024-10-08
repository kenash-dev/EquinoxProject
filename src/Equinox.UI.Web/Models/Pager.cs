namespace Equinox.UI.Web.Models;

public class Pager
{
    public int TotalItems { get; private set; }
    public int CurrentPage { get; private set; }
    public int PageSize { get; private set; }
    public int TotalPages { get; private set; }
    public int StartPage { get; private set; }
    public int EndPage { get; private set; }

    public Pager()
    {
    }

    public Pager(int totalItems, int currentPage, int pageSize = 10 )
    {
        TotalItems = totalItems;
        CurrentPage = currentPage;
        PageSize = pageSize;

        TotalPages = (int)Math.Ceiling((decimal)TotalItems/(decimal)PageSize);

        int startPage = currentPage - 3; //make this configurable
        int endPage = currentPage + 2; //make this configurable, this is displaying 5 page numbers in the display row.

        if (startPage < 0)
        {
            endPage = endPage - (startPage - 1);
            startPage = 1;
        }

        if (endPage > TotalPages)
        {
            endPage = TotalPages;
            if (endPage > 10)
            {
                startPage = endPage - 9;
            }
        }

        StartPage = startPage;
        EndPage = endPage;
    }
}