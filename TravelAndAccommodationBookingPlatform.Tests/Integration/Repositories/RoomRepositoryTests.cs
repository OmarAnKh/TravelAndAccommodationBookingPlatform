using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common.DatabaseFactories;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class RoomRepositoryTests : IDisposable
{

    private readonly IAppDbContext _context;
    private readonly IRoomRepository _roomRepository;

    public RoomRepositoryTests()
    {
        var dbContextFactory = new DbContextFactory();
        _context = dbContextFactory.Create(DatabaseType.InMemory);
        _roomRepository = new RoomRepository(_context);
    }
    

    public void Dispose()
    {
        _context.Dispose();
    }
}