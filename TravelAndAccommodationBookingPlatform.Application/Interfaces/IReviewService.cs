using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReviewService : IService<Review, ReviewQueryParameters, ReviewCreationDto, ReviewUpdateDto, ReviewDto>,IImagesService
{
    Task<ReviewDto?> CreateAsync(ReviewCreationDto entity, IFormFile file);

}