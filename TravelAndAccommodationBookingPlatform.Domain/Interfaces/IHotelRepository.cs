using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IHotelRepository : IRepository<Hotel, HotelQueryParameters>
{
    Task<Hotel?> GetByIdAsync(int id);
    Task<Hotel?> DeleteAsync(int id);
}