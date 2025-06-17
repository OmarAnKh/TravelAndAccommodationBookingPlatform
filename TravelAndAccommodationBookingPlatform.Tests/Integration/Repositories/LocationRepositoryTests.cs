using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class LocationRepositoryTests : IDisposable
{
    private readonly IAppDbContext _context;
    private readonly ILocationRepository _locationRepository;

    readonly List<Location> _locations = new List<Location>()
    {
        new Location { HotelId = 1, Latitude = 48.8566f, Longitude = 2.3522f },
        new Location { HotelId = 2, Latitude = 35.6895f, Longitude = 139.6917f },
        new Location { HotelId = 3, Latitude = 40.7128f, Longitude = -74.0060f },
        new Location { HotelId = 4, Latitude = 41.9028f, Longitude = 12.4964f },
        new Location { HotelId = 5, Latitude = 41.3851f, Longitude = 2.1734f }
    };

    public LocationRepositoryTests()
    {
        var dbFactory = new DbContextFactory();
        _context = dbFactory.Create(DatabaseType.InMemory);
        _locationRepository = new LocationRepository(_context);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnsPagedLocations_WhenNoSearchTermProvided(int pageNumber, int pageSize)
    {
        //Arrange
        _context.Locations.AddRange(_locations);
        await _context.SaveChangesAsync();
        var queryParameter = new LocationQueryParameters
        {
            Page = pageNumber,
            PageSize = pageSize
        };

        //Act
        var (entities, paginationMetaData) = await _locationRepository.GetAllAsync(queryParameter);
        var resultList = entities.ToList();

        int expectedCount = Math.Max(0, Math.Min(resultList.Count, pageSize));

        //Assert
        resultList.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Theory]
    [InlineData(5, null, null, 1, 10)]
    [InlineData(null, 41.3851f, 2.1734f, 1, 2)]
    [InlineData(null, null, null, 1, 10)]
    public async Task GetAll_WithSearchTerm_ShouldReturnFilteredLocations(int? hotelId, float? latitude, float? longitude, int pageNumber, int pageSize)
    {
        //Arrange
        _context.Locations.AddRange(_locations);
        await _context.SaveChangesAsync();
        var queryParameters = new LocationQueryParameters
        {
            Page = pageNumber,
            PageSize = pageSize,
            HotelId = hotelId,
            Latitude = latitude,
            Longitude = longitude
        };

        //Act
        var (entities, paginationMetaData) = await _locationRepository.GetAllAsync(queryParameters);
        var resultList = entities.ToList();
        var queryableResult = _locations.AsQueryable();

        if (hotelId.HasValue)
        {
            queryableResult = queryableResult.Where(location => location.HotelId == hotelId);
        }

        if (latitude.HasValue && longitude.HasValue)
        {
            queryableResult = queryableResult.Where(location => location.Latitude == latitude && location.Longitude == longitude);
        }

        var listResult = queryableResult.Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();


        //Assert
        resultList.Count.Should().Be(listResult.Count());
        resultList.Should().BeEquivalentTo(listResult);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);


    }
    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefaults()
    {
        //Arrange

        //Act
        var (entities, paginationMetaData) = await _locationRepository.GetAllAsync(new LocationQueryParameters());

        //
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnLocation()
    {
        //Arrange
        await _context.Locations.AddRangeAsync(_locations);
        await _context.SaveChangesAsync();
        var locationHotelId = _locations.First().HotelId;
        //Act
        var result = await _locationRepository.GetByIdAsync(locationHotelId);

        //Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(locationHotelId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNull(int locationHotelId)
    {
        //Arrange
        await _context.Locations.AddRangeAsync(_locations);
        await _context.SaveChangesAsync();

        //Act
        var result = await _locationRepository.GetByIdAsync(locationHotelId);

        //Assert
        result.Should().BeNull();

    }

    [Fact]
    public async Task CreateLocation_WithValidData_ShouldCreateLocation()
    {
        //Arrange
        var location = new Location { HotelId = 6, Latitude = 49.8566f, Longitude = 1.3522f };

        //Act
        var createResult = await _locationRepository.CreateAsync(location);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        createResult.Should().NotBeNull();
        saveChangesResult.Should().Be(1);

    }


    [Fact]
    public async Task DeleteLocation_WithValidData_ShouldDeleteLocation()
    {
        //Arrange
        await _context.Locations.AddRangeAsync(_locations);
        await _context.SaveChangesAsync();
        var location = await _locationRepository.GetByIdAsync(_locations[0].HotelId);

        //Act
        var deleteResult = await _locationRepository.DeleteAsync(location.HotelId);
        var saveChangesResult = await _context.SaveChangesAsync();
        var getResult = await _locationRepository.GetByIdAsync(location.HotelId);

        //Assert
        deleteResult.Should().NotBeNull();
        deleteResult.Should().BeEquivalentTo(location);
        getResult.Should().BeNull();
        saveChangesResult.Should().Be(1);

    }

    [Fact]
    public async Task DeleteLocation_WithInvalidId_ShouldReturnNull()
    {
        //Arrange
        var location = new Location { HotelId = -1, Latitude = 49.8566f, Longitude = 1.3522f };

        //Act
        var deleteResult = await _locationRepository.DeleteAsync(location.HotelId);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }


    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        //Arrange
        var location = new List<Location>
        {
            new Location { HotelId = 3, Latitude = 40.7128f, Longitude = -74.0060f },
            new Location { HotelId = 4, Latitude = 41.9028f, Longitude = 12.4964f },
        };
        await _context.Locations.AddRangeAsync(location);
        //Act
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(location.Count());
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        //Act
        var saveChangesResult = await _context.SaveChangesAsync();
        //Assert
        saveChangesResult.Should().Be(0);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}