using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IHotelService _hotelService;

    public HotelServiceTests()
    {
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _mapperMock = new Mock<IMapper>();
        _hotelService = new HotelService(_hotelRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedHotelsWithPagination()
    {
        // Arrange
        const int firstHotelId = 1;
        const int secondHotelId = 2;
        const int firstCityId = 1;
        const int secondCityId = 2;
        const string firstHotelName = "Eiffel Hotel";
        const string secondHotelName = "Shibuya Inn";
        const string firstOwner = "Anan Khalili";
        const string secondOwner = "Idk";
        const string firstDescription = "Near Eiffel Tower";
        const string secondDescription = "In the heart of Tokyo";
        const string firstThumbnail = "eiffel_hotel.jpg";
        const string secondThumbnail = "shibuya_inn.jpg";
        const int totalCount = 2;
        const int pageNumber = 1;
        const int pageSize = 10;

        var queryParameter = new HotelQueryParameters();
        var hotels = new List<Hotel>()
        {
            new Hotel { Id = firstHotelId, Name = firstHotelName, CityId = firstCityId, Owner = firstOwner, Description = firstDescription, Thumbnail = firstThumbnail },
            new Hotel { Id = secondHotelId, Name = secondHotelName, CityId = secondCityId, Owner = secondOwner, Description = secondDescription, Thumbnail = secondThumbnail }
        };
        var hotelDto = new List<HotelDto>
        {
            new HotelDto { Id = firstHotelId, Name = firstHotelName, CityId = firstCityId, Owner = firstOwner, Description = firstDescription, Thumbnail = firstThumbnail },
            new HotelDto { Id = secondHotelId, Name = secondHotelName, CityId = secondCityId, Owner = secondOwner, Description = secondDescription, Thumbnail = secondThumbnail }
        };
        var metaData = new PaginationMetaData(totalCount, pageNumber, pageSize);

        _hotelRepositoryMock.Setup(h => h.GetAll(It.IsAny<HotelQueryParameters>())).ReturnsAsync((hotels, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<HotelDto>>(hotels)).Returns(hotelDto);

        // Act
        var (result, pagination) = await _hotelService.GetAll(queryParameter);
        result = result.ToList();

        // Assert
        result.Should().HaveCount(hotelDto.Count);
        result.Should().Contain(h => h.Id == firstHotelId);
        pagination.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task Create_ShouldReturnMappedDto_WhenCreationSucceeds()
    {
        // Arrange
        const int expectedId = 1;
        const int cityId = 1;
        const string hotelName = "Eiffel Hotel";
        const string owner = "Anan Khalili";
        const string description = "Near Eiffel Tower";
        const string thumbnail = "eiffel_hotel.jpg";

        var creationDto = new HotelCreationDto()
        {
            Name = hotelName,
            CityId = cityId,
            Owner = owner,
            Description = description,
            Thumbnail = thumbnail
        };
        var hotel = new Hotel()
        {
            Name = creationDto.Name,
            CityId = creationDto.CityId,
            Owner = creationDto.Owner,
            Description = creationDto.Description,
            Thumbnail = creationDto.Thumbnail
        };
        var created = new Hotel()
        {
            Id = expectedId,
            Name = creationDto.Name,
            CityId = creationDto.CityId,
            Owner = creationDto.Owner,
            Description = creationDto.Description,
            Thumbnail = creationDto.Thumbnail
        };

        _mapperMock.Setup(h => h.Map<Hotel>(creationDto)).Returns(hotel);
        _hotelRepositoryMock.Setup(h => h.Create(hotel)).ReturnsAsync(created);
        _mapperMock.Setup(h => h.Map<HotelDto>(created)).Returns(new HotelDto()
        {
            Id = created.Id,
            Name = creationDto.Name,
            CityId = creationDto.CityId,
            Owner = creationDto.Owner,
            Description = creationDto.Description,
            Thumbnail = creationDto.Thumbnail
        });

        // Act
        var result = await _hotelService.Create(creationDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedId);
        result.Name.Should().Be(hotelName);
        result.CityId.Should().Be(cityId);
        result.Owner.Should().Be(owner);
        result.Description.Should().Be(description);
        result.Thumbnail.Should().Be(thumbnail);

    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        const string hotelName = "Idk";

        var creationDto = new HotelCreationDto() { Name = hotelName };
        var hotel = new Hotel() { Name = hotelName };

        _mapperMock.Setup(h => h.Map<Hotel>(creationDto)).Returns(hotel);
        _hotelRepositoryMock.Setup(h => h.Create(hotel)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.Create(creationDto);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(1, "Shibuya Inn")]
    [InlineData(2, "Eiffel Hotel")]
    public async Task GetById_ShouldReturnDto_WhenHotelExists(int hotelId, string hotelName)
    {
        // Arrange
        var hotel = new Hotel() { Id = hotelId, Name = hotelName };

        _hotelRepositoryMock.Setup(h => h.GetById(hotelId)).ReturnsAsync(hotel);
        _mapperMock.Setup(h => h.Map<HotelDto>(hotel)).Returns(new HotelDto() { Id = hotelId, Name = hotelName });

        // Act
        var result = await _hotelService.GetById(hotelId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hotelId);
        result.Name.Should().Be(hotelName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_ShouldReturnNull_WhenHotelDoesNotExist(int invalidId)
    {
        // Arrange
        _hotelRepositoryMock.Setup(h => h.GetById(invalidId)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.GetById(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenUpdateSucceeds()
    {
        // Arrange
        const int originalHotelId = 1;
        const int updatedHotelId = 2;
        const string updatedOwner = "Me";

        var updateDto = new HotelUpdateDto() { HotelId = originalHotelId, Owner = updatedOwner };
        var hotel = new Hotel() { Id = originalHotelId, Owner = updatedOwner };
        var updatedHotel = new Hotel() { Id = updatedHotelId, Owner = updatedOwner };

        _mapperMock.Setup(m => m.Map<Hotel>(updateDto)).Returns(hotel);
        _hotelRepositoryMock.Setup(h => h.UpdateAsync(hotel)).ReturnsAsync(updatedHotel);
        _mapperMock.Setup(m => m.Map<HotelDto>(updatedHotel)).Returns(new HotelDto() { Id = updatedHotelId, Owner = updatedOwner });

        // Act
        var result = await _hotelService.UpdateAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Owner.Should().Be(updatedOwner);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        // Arrange
        const int hotelId = 1;
        const string owner = "Me";

        var updateDto = new HotelUpdateDto() { HotelId = hotelId, Owner = owner };
        var hotel = new Hotel() { Id = hotelId, Owner = owner };

        _mapperMock.Setup(m => m.Map<Hotel>(updateDto)).Returns(hotel);
        _hotelRepositoryMock.Setup(h => h.UpdateAsync(hotel)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.UpdateAsync(updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        const int hotelId = 1;
        const string owner = "Me";

        var hotel = new Hotel() { Id = hotelId, Owner = owner };

        _hotelRepositoryMock.Setup(h => h.Delete(hotelId)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<HotelDto>(hotel)).Returns(new HotelDto() { Id = hotelId });

        // Act
        var result = await _hotelService.Delete(hotelId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hotelId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int invalidId = -1;

        _hotelRepositoryMock.Setup(h => h.Delete(It.IsAny<int>())).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.Delete(invalidId);

        // Assert
        result.Should().BeNull();
    }
}