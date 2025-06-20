namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IImagesService
{
    Task<List<string>?> GetImagesPathAsync(int id);

}