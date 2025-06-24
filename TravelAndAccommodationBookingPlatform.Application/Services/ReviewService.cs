using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IHotelRepository _hotelRepository;
    private readonly IImageUploader _imageUploader;
    private readonly IMapper _mapper;


    public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IHotelRepository hotelRepository, IImageUploader imageUploader, IMapper mapper)
    {
        _reviewRepository = reviewRepository;
        _userRepository = userRepository;
        _hotelRepository = hotelRepository;
        _imageUploader = imageUploader;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<ReviewDto>, PaginationMetaData)> GetAllAsync(ReviewQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _reviewRepository.GetAllAsync(queryParams);
        var reviews = _mapper.Map<IEnumerable<ReviewDto>>(entities);
        return (reviews, paginationMetaData);
    }

    public async Task<ReviewDto?> CreateAsync(ReviewCreationDto entity, List<IFormFile> file)
    {
        var user = await _userRepository.GetByIdAsync(entity.UserId);
        var hotel = await _hotelRepository.GetByIdAsync(entity.HotelId);
        var folderPath = await _imageUploader.UploadImagesAsync(file, ImageEntityType.Reviews);
        if (hotel is null || user is null)
            return null;

        var review = _mapper.Map<Review>(entity);
        review.CreatedAt = DateTime.UtcNow;
        review.UpdatedAt = DateTime.UtcNow;
        review.FolderPath = folderPath;
        var result = await _reviewRepository.CreateAsync(review);
        if (result is null)
            return null;

        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(result);
    }
    public async Task<ReviewDto?> UpdateAsync(int id, JsonPatchDocument<ReviewUpdateDto> patchDocument)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
        {
            return null;
        }
        var updatedReview = _mapper.Map<ReviewUpdateDto>(review);
        patchDocument.ApplyTo(updatedReview);

        _mapper.Map(updatedReview, review);
        review.UpdatedAt = DateTime.UtcNow;


        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto?> GetByIdAsync(int id)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review is null)
        {
            return null;
        }
        return _mapper.Map<ReviewDto>(review);
    }
    public async Task<ReviewDto?> DeleteAsync(int id)
    {
        var review = await _reviewRepository.DeleteAsync(id);
        if (review is null)
        {
            return null;
        }
        await _reviewRepository.SaveChangesAsync();
        return _mapper.Map<ReviewDto>(review);
    }
    public async Task<List<string>?> GetImagesPathAsync(int id)
    {
        var hotel = await _hotelRepository.GetByIdAsync(id);
        if (hotel is null)
        {
            return null;
        }
        var images = await _imageUploader.GetImageUrlsAsync(hotel.FolderPath);
        return images;
    }
}