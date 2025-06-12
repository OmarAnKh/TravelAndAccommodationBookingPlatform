using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReviewService : IService<Review, ReviewQueryParameters>
{
    Task<Review?> GetById(int id);
    Task<Review?> Delete(int id);
}