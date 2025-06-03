namespace TravelAndAccommodationBookingPlatform.Domain.Common;

public class PaginationMetaData
{
    public int TotalCount { get; set; }
    public int TotalPageCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }

    public PaginationMetaData(int totalItemCount, int currentPage, int pageSize)
    {
        TotalCount = totalItemCount;
        PageSize = pageSize;
        CurrentPage = currentPage;
        TotalPageCount = (int)Math.Ceiling(totalItemCount / (double)pageSize);
    }
}