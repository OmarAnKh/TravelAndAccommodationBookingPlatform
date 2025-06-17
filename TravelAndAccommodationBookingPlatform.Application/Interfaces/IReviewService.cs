using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IReviewService : IService<Review, ReviewQueryParameters, ReviewCreationDto, ReviewUpdateDto, ReviewDto>
{
    Task<ReviewDto?> GetByIdAsync(int id);
    Task<ReviewDto?> DeleteAsync(int id);
    Task<ReviewDto?> CreateAsync(ReviewCreationDto entity, IFormFile file);
    Task<ReviewDto?> UpdateAsync(int id, JsonPatchDocument<ReviewUpdateDto> patchDocument);

}