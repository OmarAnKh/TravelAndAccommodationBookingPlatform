using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Interfaces;

public interface ITestDbContextFactory
{
    IAppDbContext Create();

}