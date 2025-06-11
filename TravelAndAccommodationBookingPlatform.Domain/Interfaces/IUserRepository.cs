using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IUserRepository : IRepository<User, UserQueryParameters>
{
    Task<User?> GetById(int id);
    Task<User?> Delete(int id);
}