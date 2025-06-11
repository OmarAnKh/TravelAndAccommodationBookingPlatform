using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IReviewRepository : IRepository<Review, ReviewQueryParameters>
{
    Task<Review?> GetById(int id);
    Task<Review?> Delete(int id);
}