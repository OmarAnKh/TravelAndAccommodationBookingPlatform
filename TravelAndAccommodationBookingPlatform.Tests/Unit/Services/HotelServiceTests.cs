using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Hotel;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class HotelServiceTests
{
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<ICityRepository> _cityRepositoryMock;
    private readonly Mock<IImageUploader> _imageUploaderMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly IHotelService _hotelService;

    public HotelServiceTests()
    {
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _cityRepositoryMock = new Mock<ICityRepository>();
        _mapperMock = new Mock<IMapper>();
        _imageUploaderMock = new Mock<IImageUploader>();
        _hotelService = new HotelService(_hotelRepositoryMock.Object, _cityRepositoryMock.Object, _imageUploaderMock.Object, _mapperMock.Object);
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

        _hotelRepositoryMock.Setup(h => h.GetAllAsync(It.IsAny<HotelQueryParameters>())).ReturnsAsync((hotels, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<HotelDto>>(hotels)).Returns(hotelDto);

        // Act
        var (result, pagination) = await _hotelService.GetAllAsync(queryParameter);
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
        const string thumbnail = "uploaded_image.jpg";

        var creationDto = new HotelCreationDto
        {
            Name = hotelName,
            CityId = cityId,
            Owner = owner,
            Description = description,
        };

        var hotel = new Hotel
        {
            Name = hotelName,
            CityId = cityId,
            Owner = owner,
            Description = description,
        };

        var createdHotel = new Hotel
        {
            Id = expectedId,
            Name = hotelName,
            CityId = cityId,
            Owner = owner,
            Description = description,
            Thumbnail = thumbnail
        };

        _mapperMock.Setup(m => m.Map<Hotel>(creationDto)).Returns(hotel);
        _cityRepositoryMock.Setup(c => c.GetByIdAsync(cityId)).ReturnsAsync(new City());
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Hotels)).ReturnsAsync("some/image.jpg");
        _hotelRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Hotel>())).ReturnsAsync(createdHotel);
        _mapperMock.Setup(m => m.Map<HotelDto>(createdHotel)).Returns(new HotelDto
        {
            Id = expectedId,
            Name = hotelName,
            CityId = cityId,
            Owner = owner,
            Description = description,
            Thumbnail = thumbnail
        });

        // Act
        var result = await _hotelService.CreateAsync(creationDto, new List<IFormFile>());

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
    public async Task Create_ShouldReturnNull_WhenCityDoesNotExistsNull()
    {
        // Arrange
        const string hotelName = "Idk";

        var creationDto = new HotelCreationDto { Name = hotelName };

        _cityRepositoryMock.Setup(c => c.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((City?)null);

        // Act
        var result = await _hotelService.CreateAsync(creationDto, new List<IFormFile>());

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenHotelRepositoryReturnsNull()
    {
        // Arrange
        const string hotelName = "Idk";

        var creationDto = new HotelCreationDto { Name = hotelName };
        var mappedHotel = new Hotel { Name = hotelName };


        _mapperMock.Setup(m => m.Map<Hotel>(creationDto)).Returns(mappedHotel);
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Hotels)).ReturnsAsync("some/image.jpg");

        _hotelRepositoryMock.Setup(h => h.CreateAsync(It.IsAny<Hotel>())).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.CreateAsync(creationDto, new List<IFormFile>());

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

        _hotelRepositoryMock.Setup(h => h.GetByIdAsync(hotelId)).ReturnsAsync(hotel);
        _mapperMock.Setup(h => h.Map<HotelDto>(hotel)).Returns(new HotelDto() { Id = hotelId, Name = hotelName });

        // Act
        var result = await _hotelService.GetByIdAsync(hotelId);

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
        _hotelRepositoryMock.Setup(h => h.GetByIdAsync(invalidId)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }
    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenUpdateSucceeds()
    {
        // Arrange
        const int hotelId = 1;
        const string originalOwner = "Original Owner";
        const string updatedOwner = "Me";

        var existingHotel = new Hotel { Id = hotelId, Owner = originalOwner };
        var hotelUpdateDto = new HotelUpdateDto { Owner = updatedOwner };
        var expectedHotelDto = new HotelDto { Id = hotelId, Owner = updatedOwner };

        var patchDoc = new JsonPatchDocument<HotelUpdateDto>();
        patchDoc.Replace(x => x.Owner, updatedOwner);

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotelId)).ReturnsAsync(existingHotel);
        _mapperMock.Setup(m => m.Map<HotelUpdateDto>(existingHotel)).Returns(hotelUpdateDto);
        _mapperMock.Setup(m => m.Map(hotelUpdateDto, existingHotel));
        _mapperMock.Setup(m => m.Map<HotelDto>(existingHotel)).Returns(expectedHotelDto);
        _hotelRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _hotelService.UpdateAsync(hotelId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Owner.Should().Be(updatedOwner);
        _hotelRepositoryMock.Verify(r => r.GetByIdAsync(hotelId), Times.Once);
        _hotelRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenHotelNotFound()
    {
        // Arrange
        const int hotelId = 1;
        const string owner = "Me";

        var patchDoc = new JsonPatchDocument<HotelUpdateDto>();
        patchDoc.Replace(x => x.Owner, owner);

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotelId)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.UpdateAsync(hotelId, patchDoc);

        // Assert
        result.Should().BeNull();
        _hotelRepositoryMock.Verify(r => r.GetByIdAsync(hotelId), Times.Once);
        _hotelRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldApplyPatchCorrectly_WhenMultipleFieldsUpdated()
    {
        // Arrange
        const int hotelId = 2;
        const string originalOwner = "Original Owner";
        const string updatedOwner = "Updated Owner";

        var existingHotel = new Hotel { Id = hotelId, Owner = originalOwner };
        var hotelUpdateDto = new HotelUpdateDto { Owner = updatedOwner };
        var expectedHotelDto = new HotelDto { Id = hotelId, Owner = updatedOwner };

        var patchDoc = new JsonPatchDocument<HotelUpdateDto>();
        patchDoc.Replace(x => x.Owner, updatedOwner);

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(hotelId)).ReturnsAsync(existingHotel);
        _mapperMock.Setup(m => m.Map<HotelUpdateDto>(existingHotel)).Returns(hotelUpdateDto);
        _mapperMock.Setup(m => m.Map(hotelUpdateDto, existingHotel));
        _mapperMock.Setup(m => m.Map<HotelDto>(existingHotel)).Returns(expectedHotelDto);
        _hotelRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _hotelService.UpdateAsync(hotelId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Owner.Should().Be(updatedOwner);
    }
    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        const int hotelId = 1;
        const string owner = "Me";

        var hotel = new Hotel() { Id = hotelId, Owner = owner };

        _hotelRepositoryMock.Setup(h => h.DeleteAsync(hotelId)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<HotelDto>(hotel)).Returns(new HotelDto() { Id = hotelId });

        // Act
        var result = await _hotelService.DeleteAsync(hotelId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(hotelId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int invalidId = -1;

        _hotelRepositoryMock.Setup(h => h.DeleteAsync(It.IsAny<int>())).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _hotelService.DeleteAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }
}