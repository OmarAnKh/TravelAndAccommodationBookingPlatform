using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IService<T, TParams> where T : class where TParams : IQueryParameters
{
    Task<(IEnumerable<T>, PaginationMetaData)> GetAll(TParams queryParams);
    Task<T?> Create(T entity);
    Task<T?> UpdateAsync(T entity);
}