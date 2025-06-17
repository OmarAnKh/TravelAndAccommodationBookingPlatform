using Microsoft.AspNetCore.Http;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IImageUploader
{
    Task<string> UploadImagesAsync(List<IFormFile> files, ImageEntityType imageType);
    Task DeleteImageAsync(string imageUrl);
}