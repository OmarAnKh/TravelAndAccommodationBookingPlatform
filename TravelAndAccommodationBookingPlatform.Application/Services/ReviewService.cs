using AutoMapper;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IMapper _mapper;

    public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IHotelRepository hotelRepository, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _hotelRepository = hotelRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ReviewDto>, PaginationMetaData)> GetAll(ReviewQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _reviewRepository.GetAll(queryParams);
        var reviews = _mapper.Map<IEnumerable<ReviewDto>>(entities);
        return (reviews, paginationMetaData);
    }
    public async Task<ReviewDto?> Create(ReviewCreationDto entity)
    {
        var user = await _userRepository.GetById(entity.UserId);
        var hotel = await _hotelRepository.GetById(entity.HotelId);
        if (hotel is null || user is null)
            return null;

        var review = _mapper.Map<Review>(entity);
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;

        var result = await _reviewRepository.Create(review);
        if (result is null)
            return null;

        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(result);
    }

    public async Task<ReviewDto?> UpdateAsync(ReviewUpdateDto entity)
    {
        var review = _mapper.Map<Review>(entity);
        review.UpdatedAt = DateTime.UtcNow;
        var result = await _reviewRepository.UpdateAsync(review);
        if (result is null)
        {
            return null;
        }
        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(result);
    }
    public async Task<ReviewDto?> GetById(int id)
    {
        var review = await _reviewRepository.GetById(id);
        if (review is null)
        {
            return null;
        }
        return _mapper.Map<ReviewDto>(review);
    }
    public async Task<ReviewDto?> Delete(int id)
    {
        var review = await _reviewRepository.Delete(id);
        if (review is null)
        {
            return null;
        }
        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(review);
    }
}