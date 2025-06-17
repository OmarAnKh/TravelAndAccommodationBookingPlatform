using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class CityServiceTests
{
    private readonly Mock<ICityRepository> _cityRepoMock;
    private readonly Mock<IImageUploader> _imageUploaderMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CityService _cityService;

    public CityServiceTests()
    {
        _cityRepoMock = new Mock<ICityRepository>();
        _mapperMock = new Mock<IMapper>();
        _imageUploaderMock = new Mock<IImageUploader>();
        _cityService = new CityService(_cityRepoMock.Object, _mapperMock.Object, _imageUploaderMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedCitiesWithPagination()
    {
        // Arrange
        const int cityId = 1;
        const string cityName = "Ramallah";
        const int totalCount = 1;
        const int pageNumber = 1;
        const int pageSize = 10;
        const int expectedCount = 1;

        var queryParams = new CityQueryParameters();
        var cities = new List<City> { new City() { Id = cityId, Name = cityName } };
        var metaData = new PaginationMetaData(totalCount, pageNumber, pageSize);

        _cityRepoMock.Setup(r => r.GetAllAsync(queryParams)).ReturnsAsync((cities, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<CityDto>>(cities)).Returns(new List<CityDto>
        {
            new CityDto() { Id = cityId, Name = cityName }
        });

        // Act
        var (result, pagination) = await _cityService.GetAllAsync(queryParams);
        result = result.ToList();

        // Assert
        result.Should().HaveCount(expectedCount);
        result.Should().ContainSingle(c => c.Name == cityName);
        pagination.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task Create_ShouldReturnMappedDto_WhenCreationSucceeds()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        const int expectedId = 10;
        const string cityName = "Jericho";

        var creationDto = new CityCreationDto
        {
            Name = cityName,
            Thumbnail = "jericho.jpg",
            Country = "Palestine",
            PostOffice = "12345"
        };

        var city = new City
        {
            Name = cityName,
            Thumbnail = creationDto.Thumbnail,
            Country = creationDto.Country,
            PostOffice = creationDto.PostOffice
        };

        var created = new City
        {
            Id = expectedId,
            Name = cityName,
            Thumbnail = city.Thumbnail,
            Country = city.Country,
            PostOffice = city.PostOffice
        };

        var expectedDto = new CityDto
        {
            Id = expectedId,
            Name = cityName,
            Thumbnail = created.Thumbnail,
            Country = created.Country,
            PostOffice = created.PostOffice
        };

        _mapperMock.Setup(m => m.Map<City>(creationDto)).Returns(city);
        _cityRepoMock.Setup(r => r.CreateAsync(city)).ReturnsAsync(created);
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Cities)).ReturnsAsync("some/image.jpg");
        _mapperMock.Setup(m => m.Map<CityDto>(created)).Returns(expectedDto);

        // Act
        var result = await _cityService.CreateAsync(creationDto, fileMock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDto);
    }


    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var creationDto = new CityCreationDto { Name = "FailCity" };
        var city = new City { Name = "FailCity" };

        _mapperMock.Setup(m => m.Map<City>(creationDto)).Returns(city);
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Cities)).ReturnsAsync("some/image.jpg");
        _cityRepoMock.Setup(r => r.CreateAsync(city)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.CreateAsync(creationDto, fileMock.Object);

        // Assert
        result.Should().BeNull();
    }


    [Theory]
    [InlineData(5, "Nablus")]
    [InlineData(6, "Bethlehem")]
    public async Task GetById_ShouldReturnDto_WhenCityExists(int cityId, string cityName)
    {
        // Arrange
        var city = new City() { Id = cityId, Name = cityName };

        _cityRepoMock.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = cityId, Name = cityName });

        // Act
        var result = await _cityService.GetByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cityId);
        result.Name.Should().Be(cityName);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_ShouldReturnNull_WhenCityDoesNotExist(int invalidId)
    {
        // Arrange
        _cityRepoMock.Setup(r => r.GetByIdAsync(invalidId)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.GetByIdAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        // Arrange
        const int cityId = 2;
        const string originalCityName = "Original City";
        const string updatedCityName = "Updated City";

        var existingCity = new City { Id = cityId, Name = originalCityName };
        var cityUpdateDto = new CityUpdateDto { CityId = cityId, Name = updatedCityName };
        var expectedCityDto = new CityDto { Id = cityId, Name = updatedCityName };

        var patchDoc = new JsonPatchDocument<CityUpdateDto>();
        patchDoc.Replace(x => x.Name, updatedCityName);

        _cityRepoMock.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync(existingCity);
        _mapperMock.Setup(m => m.Map<CityUpdateDto>(existingCity)).Returns(cityUpdateDto);
        _mapperMock.Setup(m => m.Map(existingCity, cityUpdateDto));
        _mapperMock.Setup(m => m.Map<CityDto>(existingCity)).Returns(expectedCityDto);
        _cityRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _cityService.UpdateAsync(cityId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updatedCityName);
        _cityRepoMock.Verify(r => r.GetByIdAsync(cityId), Times.Once);
        _cityRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenCityNotFound()
    {
        // Arrange
        const int cityId = 3;
        const string cityName = "Non-existent City";

        var patchDoc = new JsonPatchDocument<CityUpdateDto>();
        patchDoc.Replace(x => x.Name, cityName);

        _cityRepoMock.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.UpdateAsync(cityId, patchDoc);

        // Assert
        result.Should().BeNull();
        _cityRepoMock.Verify(r => r.GetByIdAsync(cityId), Times.Once);
        _cityRepoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldApplyPatchCorrectly_WhenMultipleFieldsUpdated()
    {
        // Arrange
        const int cityId = 4;
        const string originalName = "Original City";
        const string updatedName = "Updated City";

        var existingCity = new City { Id = cityId, Name = originalName };
        var cityUpdateDto = new CityUpdateDto { CityId = cityId, Name = updatedName };
        var expectedCityDto = new CityDto { Id = cityId, Name = updatedName };

        var patchDoc = new JsonPatchDocument<CityUpdateDto>();
        patchDoc.Replace(x => x.Name, updatedName);
        patchDoc.Replace(x => x.CityId, cityId);

        _cityRepoMock.Setup(r => r.GetByIdAsync(cityId)).ReturnsAsync(existingCity);
        _mapperMock.Setup(m => m.Map<CityUpdateDto>(existingCity)).Returns(cityUpdateDto);
        _mapperMock.Setup(m => m.Map(existingCity, cityUpdateDto));
        _mapperMock.Setup(m => m.Map<CityDto>(existingCity)).Returns(expectedCityDto);
        _cityRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _cityService.UpdateAsync(cityId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updatedName);
    }


    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        const int cityId = 4;
        const string cityName = "To Be Deleted";

        var city = new City() { Id = cityId, Name = cityName };

        _cityRepoMock.Setup(r => r.DeleteAsync(cityId)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = cityId, Name = cityName });

        // Act
        var result = await _cityService.DeleteAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cityId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int invalidId = -1;

        _cityRepoMock.Setup(r => r.DeleteAsync(It.IsAny<int>())).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.DeleteAsync(invalidId);

        // Assert
        result.Should().BeNull();
    }
}