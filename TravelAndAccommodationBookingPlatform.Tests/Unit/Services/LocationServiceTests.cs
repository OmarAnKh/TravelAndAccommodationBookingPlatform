using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Location;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class LocationServiceTests
{
    private readonly Mock<ILocationRepository> _locationRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ILocationService _locationService;

    public LocationServiceTests()
    {
        _locationRepositoryMock = new Mock<ILocationRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _mapperMock = new Mock<IMapper>();
        _locationService = new LocationService(_locationRepositoryMock.Object, _hotelRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedLocationWithPagination()
    {
        //Arrange
        const int locationHotelId = 1;
        const float latitude = 43.6f;
        const float longitude = -73.3f;
        const int totalCount = 1;
        const int pageNumber = 1;
        const int pageSize = 10;
        const int expectedCount = 1;

        var queryParameter = new LocationQueryParameters();
        var locations = new List<Location>() { new Location() { HotelId = locationHotelId, Latitude = latitude, Longitude = longitude } };
        var metaData = new PaginationMetaData(totalCount, pageNumber, pageSize);

        _locationRepositoryMock.Setup(l => l.GetAll(queryParameter)).ReturnsAsync((locations, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<LocationDto>>(locations)).Returns(new List<LocationDto>()
        {

            new LocationDto() { HotelId = locationHotelId, Latitude = latitude, Longitude = longitude }
        });

        //Act
        var (result, pagination) = await _locationService.GetAll(queryParameter);
        result = result.ToList();

        //Assert
        result.Should().HaveCount(expectedCount);
        result.Should().ContainSingle(l => l.HotelId == locationHotelId);
        pagination.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task Create_ShouldReturnMappedDto_WhenCreationSucceeds()
    {
        //Arrange
        const int expectedHotelId = 1;
        const float latitude = 43.6f;
        const float longitude = -73.3f;

        var creationDto = new LocationCreationDto()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        };
        var location = new Location() { HotelId = expectedHotelId, Latitude = latitude, Longitude = longitude };
        _hotelRepositoryMock.Setup(h => h.GetById(expectedHotelId)).ReturnsAsync(new Hotel() { Id = expectedHotelId });
        _mapperMock.Setup(m => m.Map<Location>(creationDto)).Returns(location);
        _locationRepositoryMock.Setup(l => l.Create(location)).ReturnsAsync(location);
        _mapperMock.Setup(m => m.Map<LocationDto>(location)).Returns(new LocationDto()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        });

        //Act
        var result = await _locationService.Create(creationDto);

        //Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(expectedHotelId);
        result.Latitude.Should().Be(latitude);
        result.Longitude.Should().Be(longitude);

    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenLocationRepositoryReturnsNull()
    {
        //Arrange
        const int expectedHotelId = 1;
        var creationDto = new LocationCreationDto()
        {
            HotelId = expectedHotelId,
            Latitude = 43.6f,
            Longitude = -10.5f
        };
        var location = new Location() { HotelId = expectedHotelId, Latitude = 43.6f };
        _mapperMock.Setup(m => m.Map<Location>(creationDto)).Returns(location);
        _hotelRepositoryMock.Setup(h => h.GetById(expectedHotelId)).ReturnsAsync(new Hotel() { Id = expectedHotelId });
        _locationRepositoryMock.Setup(l => l.Create(location)).ReturnsAsync((Location?)null);

        //Act
        var result = await _locationService.Create(creationDto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenHotelRepositoryReturnsNull()
    {
        //Arrange
        const int expectedHotelId = 1;
        var creationDto = new LocationCreationDto()
        {
            HotelId = expectedHotelId,
            Latitude = 43.6f,
            Longitude = -10.5f
        };
        var location = new Location() { HotelId = expectedHotelId, Latitude = 43.6f };
        _mapperMock.Setup(m => m.Map<Location>(creationDto)).Returns(location);
        _hotelRepositoryMock.Setup(h => h.GetById(expectedHotelId)).ReturnsAsync((Hotel?)null);

        //Act
        var result = await _locationService.Create(creationDto);

        //Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1, 43.5f, 10.5f)]
    [InlineData(2, 43.6f, -1f)]
    public async Task GetById_ShouldReturnDto_WhenLocationExists(int hotelId, float latitude, float longitude)
    {
        //Arrange
        var location = new Location() { HotelId = hotelId, Latitude = latitude, Longitude = longitude };

        _locationRepositoryMock.Setup(l => l.GetById(hotelId)).ReturnsAsync(location);
        _mapperMock.Setup(m => m.Map<LocationDto>(location)).Returns(new LocationDto()
        {
            HotelId = hotelId,
            Latitude = latitude,
            Longitude = longitude
        });

        //Act
        var result = await _locationService.GetById(hotelId);

        //Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(hotelId);
        result.Latitude.Should().Be(latitude);
        result.Longitude.Should().Be(longitude);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_ShouldReturnNull_WhenLocationDoesNotExist(int invalidId)
    {
        //Arrange
        _locationRepositoryMock.Setup(l => l.GetById(invalidId)).ReturnsAsync((Location?)null);

        //Act
        var result = await _locationService.GetById(invalidId);

        //
        result.Should().BeNull();
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        //Arrange
        const int expectedHotelId = 1;
        const float latitude = 43.6f;
        const float longitude = -10.5f;
        const float updatedLatitude = 45.6f;
        var updateDto = new LocationUpdateDto()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        };
        var location = new Location()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        };
        var updateLocation = new Location()
        {
            HotelId = expectedHotelId,
            Latitude = updatedLatitude
        };
        _mapperMock.Setup(m => m.Map<Location>(updateDto)).Returns(location);
        _locationRepositoryMock.Setup(l => l.UpdateAsync(location)).ReturnsAsync(updateLocation);
        _mapperMock.Setup(m => m.Map<LocationDto>(updateLocation)).Returns(new LocationDto()
        {
            HotelId = expectedHotelId,
            Latitude = updatedLatitude,
            Longitude = longitude
        });

        //Act
        var result = await _locationService.UpdateAsync(updateDto);

        //Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(expectedHotelId);
        result.Latitude.Should().Be(updatedLatitude);
        result.Longitude.Should().Be(longitude);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        //Arrange
        const int expectedHotelId = 1;
        const float latitude = 43.6f;
        const float longitude = -10.5f;

        var updateDto = new LocationUpdateDto()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        };
        var location = new Location()
        {
            HotelId = expectedHotelId,
        };
        _mapperMock.Setup(m => m.Map<Location>(updateDto)).Returns(location);
        _locationRepositoryMock.Setup(l => l.UpdateAsync(location)).ReturnsAsync((Location?)null);

        //Act
        var result = await _locationService.UpdateAsync(updateDto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        //Arrange
        const int expectedHotelId = 1;
        const float latitude = 43.6f;
        const float longitude = -10.5f;

        var location = new Location()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        };

        _locationRepositoryMock.Setup(l => l.Delete(expectedHotelId)).ReturnsAsync(location);
        _mapperMock.Setup(m => m.Map<LocationDto>(location)).Returns(new LocationDto()
        {
            HotelId = expectedHotelId,
            Latitude = latitude,
            Longitude = longitude
        });

        //Act
        var result = await _locationService.Delete(expectedHotelId);

        //Assert
        result.Should().NotBeNull();
        result.HotelId.Should().Be(expectedHotelId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        //Arrange
        const int expectedHotelId = 1;

        _locationRepositoryMock.Setup(l => l.Delete(expectedHotelId)).ReturnsAsync((Location?)null);

        //Act
        var result = await _locationService.Delete(expectedHotelId);

        //Assert
        result.Should().BeNull();
    }
}