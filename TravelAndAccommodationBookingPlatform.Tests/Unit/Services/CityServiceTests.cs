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
        var queryParams = new CityQueryParameters();
        var cities = new List<City> { new() { Id = 1, Name = "Ramallah" } };
        var metaData = new PaginationMetaData(1, 10, 1);

        _cityRepoMock.Setup(r => r.GetAll(queryParams)).ReturnsAsync((cities, metaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<CityDto>>(cities)).Returns(new List<CityDto>
        {
            new() { Id = 1, Name = "Ramallah" }
        });

        // Act
        var (result, pagination) = await _cityService.GetAll(queryParams);

        // Assert
        result.Should().HaveCount(1);
        result.Should().ContainSingle(c => c.Name == "Ramallah");
        pagination.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task Create_ShouldReturnMappedDto_WhenCreationSucceeds()
    {
        // Arrange
        var creationDto = new CityCreationDto
        {
            Name = "Jericho",
            Thumbnail = "jericho.jpg",
            Country = "Palestine",
            PostOffice = "12345",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
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
            Id = 10,
            Name = "Jericho",
            Thumbnail = "jericho.jpg",
            Country = "Palestine",
            PostOffice = "12345",
            CreatedAt = creationDto.CreatedAt,
            UpdatedAt = creationDto.UpdatedAt
        };

        _mapperMock.Setup(m => m.Map<City>(creationDto)).Returns(city);
        _cityRepoMock.Setup(r => r.Create(city)).ReturnsAsync(created);
        _mapperMock.Setup(m => m.Map<CityDto>(created)).Returns(new CityDto
        {
            Id = 10,
            Name = "Jericho",
            Thumbnail = "jericho.jpg",
            Country = "Palestine",
            PostOffice = "12345",
            CreatedAt = creationDto.CreatedAt,
            UpdatedAt = creationDto.UpdatedAt
        });

        // Act
        var result = await _cityService.Create(creationDto);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(10);
        result.Name.Should().Be("Jericho");
        result.Thumbnail.Should().Be("jericho.jpg");
        result.Country.Should().Be("Palestine");
        result.PostOffice.Should().Be("12345");
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        // Arrange
        var creationDto = new CityCreationDto { Name = "FailCity" };
        var city = new City { Name = "FailCity" };

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
    public async Task GetById_ShouldReturnDto_WhenCityExists(int id, string name)
    {
        // Arrange
        var city = new City { Id = id, Name = name };

        _cityRepoMock.Setup(r => r.GetById(id)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = id, Name = name });

        // Act
        var result = await _cityService.GetById(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be(name);
    }

    [Theory]
    [InlineData(999)]
    public async Task GetById_ShouldReturnNull_WhenCityDoesNotExist(int id)
    {
        _cityRepoMock.Setup(r => r.GetById(id)).ReturnsAsync((City?)null);

        var result = await _cityService.GetById(id);

        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        // Arrange
        var updateDto = new CityUpdateDto { CityId = 2, Name = "Updated City" };
        var city = new City { Id = 2, Name = "Updated City" };
        var updated = new City { Id = 2, Name = "Updated City" };

        _mapperMock.Setup(m => m.Map<City>(updateDto)).Returns(city);
        _cityRepoMock.Setup(r => r.UpdateAsync(city)).ReturnsAsync(updated);
        _mapperMock.Setup(m => m.Map<CityDto>(updated)).Returns(new CityDto { Id = 2, Name = "Updated City" });

        // Act
        var result = await _cityService.UpdateAsync(updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated City");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        var updateDto = new CityUpdateDto { CityId = 3, Name = "Fail Update" };
        var city = new City { Id = 3, Name = "Fail Update" };

        _mapperMock.Setup(m => m.Map<City>(updateDto)).Returns(city);
        _cityRepoMock.Setup(r => r.UpdateAsync(city)).ReturnsAsync((City?)null);

        var result = await _cityService.UpdateAsync(updateDto);

        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        var id = 4;
        var city = new City { Id = 4, Name = "To Be Deleted" };

        _cityRepoMock.Setup(r => r.Delete(id)).ReturnsAsync(city);
        _mapperMock.Setup(m => m.Map<CityDto>(city)).Returns(new CityDto { Id = 4, Name = "To Be Deleted" });

        // Act
        var result = await _cityService.Delete(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(4);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenCityNotFound()
    {
        _cityRepoMock.Setup(r => r.Delete(It.IsAny<int>())).ReturnsAsync((City?)null);

        var result = await _cityService.Delete(999);

        result.Should().BeNull();
    }
}