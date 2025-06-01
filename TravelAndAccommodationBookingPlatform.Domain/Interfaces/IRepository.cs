using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IRepository<T> where T : class
{
    Task<(IEnumerable<T>, PaginationMetaData)> GetAll(IQueryParameters parameters);
    Task<T?> GetById(int id);
    Task<T?> Create(T entity);
    T Update(T entity);
    Task<T?> Delete(int id);
    Task<int> SaveChanges();
}