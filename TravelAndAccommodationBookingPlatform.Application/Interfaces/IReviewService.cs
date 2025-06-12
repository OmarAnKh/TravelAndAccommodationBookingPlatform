using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReviewService : IService<Review, ReviewQueryParameters, ReviewCreationDto, ReviewUpdateDto, ReviewDto>
{
    Task<Review?> GetById(int id);
    Task<Review?> Delete(int id);
}