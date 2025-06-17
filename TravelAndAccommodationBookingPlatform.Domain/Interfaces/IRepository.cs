using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRepository<T, TParams> where T : class where TParams : IQueryParameters
{
    Task<(IEnumerable<T>, PaginationMetaData)> GetAllAsync(TParams queryParams);
    Task<T?> CreateAsync(T entity);
    Task<int> SaveChangesAsync();
}