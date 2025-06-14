using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.API.Profiles;

public class ReviewProfile : Profile
{
    public ReviewProfile()
    {
        CreateMap<Review, ReviewDto>();
        CreateMap<ReviewCreationDto, Review>();
        CreateMap<ReviewUpdateDto, Review>();
    }
}