using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRepository<T, TParams> where T : class where TParams : IQueryParameters
{
    Task<(IEnumerable<T>, PaginationMetaData)> GetAll(TParams queryParams);
    Task<T?> Create(T entity);
    Task<T?> UpdateAsync(T entity);
    Task<int> SaveChangesAsync();
}