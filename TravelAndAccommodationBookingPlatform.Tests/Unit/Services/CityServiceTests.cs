using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.City;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class CityServiceTests
{
    private readonly Mock<ICityRepository> _cityRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CityService _cityService;

    public CityServiceTests()
    {
        _cityRepoMock = new Mock<ICityRepository>();
        _mapperMock = new Mock<IMapper>();
        _cityService = new CityService(_cityRepoMock.Object, _mapperMock.Object);
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

        _cityRepoMock.Setup(r => r.GetAll(queryParams)).ReturnsAsync((cities, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<CityDto>>(cities)).Returns(new List<CityDto>
        {
            new CityDto() { Id = cityId, Name = cityName }
        });

        // Act
        var (result, pagination) = await _cityService.GetAll(queryParams);
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
        const int expectedId = 10;
        const string cityName = "Jericho";
        const string thumbnail = "jericho.jpg";
        const string country = "Palestine";
        const string postOffice = "12345";
        var createdAt = DateTime.Now;
        var updatedAt = DateTime.Now;

        var creationDto = new CityCreationDto
        {
            Name = cityName,
            Thumbnail = thumbnail,
            Country = country,
            PostOffice = postOffice,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };

        var city = new City
        {
            Name = creationDto.Name,
            Thumbnail = creationDto.Thumbnail,
            Country = creationDto.Country,
            PostOffice = creationDto.PostOffice,
            CreatedAt = creationDto.CreatedAt,
            UpdatedAt = creationDto.UpdatedAt
        };

        var created = new City
        {
            Id = expectedId,
            Name = cityName,
            Thumbnail = thumbnail,
            Country = country,
            PostOffice = postOffice,
            CreatedAt = creationDto.CreatedAt,
            UpdatedAt = creationDto.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<City>(creationDto)).Returns(city);
        _cityRepoMock.Setup(r => r.Create(city)).ReturnsAsync(created);
        _mapperMock.Setup(m => m.Map<CityDto>(created)).Returns(new CityDto
        {
            Id = expectedId,
            Name = cityName,
            Thumbnail = thumbnail,
            Country = country,
            PostOffice = postOffice,
            CreatedAt = creationDto.CreatedAt,
            UpdatedAt = creationDto.UpdatedAt
        });

        // Act
        var result = await _cityService.Create(creationDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(expectedId);
        result.Name.Should().Be(cityName);
        result.Thumbnail.Should().Be(thumbnail);
        result.Country.Should().Be(country);
        result.PostOffice.Should().Be(postOffice);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        const string cityName = "FailCity";

        var creationDto = new CityCreationDto { Name = cityName };
        var city = new City { Name = cityName };

        _mapperMock.Setup(m => m.Map<City>(creationDto)).Returns(city);
        _cityRepoMock.Setup(r => r.Create(city)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.Create(creationDto);

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

        _cityRepoMock.Setup(r => r.GetById(cityId)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = cityId, Name = cityName });

        // Act
        var result = await _cityService.GetById(cityId);

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
        _cityRepoMock.Setup(r => r.GetById(invalidId)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.GetById(invalidId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        // Arrange
        const int cityId = 2;
        const string updatedCityName = "Updated City";

        var updateDto = new CityUpdateDto() { CityId = cityId, Name = updatedCityName };
        var city = new City() { Id = cityId, Name = updatedCityName };
        var updated = new City() { Id = cityId, Name = updatedCityName };

        _mapperMock.Setup(m => m.Map<City>(updateDto)).Returns(city);
        _cityRepoMock.Setup(r => r.UpdateAsync(city)).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<CityDto>(updated)).Returns(new CityDto { Id = cityId, Name = updatedCityName });

        // Act
        var result = await _cityService.UpdateAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updatedCityName);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        // Arrange
        const int cityId = 3;
        const string cityName = "Fail Update";

        var updateDto = new CityUpdateDto { CityId = cityId, Name = cityName };
        var city = new City { Id = cityId, Name = cityName };

        _mapperMock.Setup(m => m.Map<City>(updateDto)).Returns(city);
        _cityRepoMock.Setup(r => r.UpdateAsync(city)).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.UpdateAsync(updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        const int cityId = 4;
        const string cityName = "To Be Deleted";

        var city = new City() { Id = cityId, Name = cityName };

        _cityRepoMock.Setup(r => r.Delete(cityId)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = cityId, Name = cityName });

        // Act
        var result = await _cityService.Delete(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cityId);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int invalidId = -1;

        _cityRepoMock.Setup(r => r.Delete(It.IsAny<int>())).ReturnsAsync((City?)null);

        // Act
        var result = await _cityService.Delete(invalidId);

        // Assert
        result.Should().BeNull();
    }
}