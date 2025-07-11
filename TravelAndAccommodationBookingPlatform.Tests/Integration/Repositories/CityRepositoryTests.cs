using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class CityRepositoryTests : IDisposable
{
    private readonly IAppDbContext _context;
    private readonly ICityRepository _cityRepository;

    readonly List<City> _cities = new List<City>
    {
        new City { Name = "Paris", Country = "France", FolderPath = "paris.jpg", PostOffice = "75000" },
        new City { Name = "Tokyo", Country = "Japan", FolderPath = "tokyo.jpg", PostOffice = "100-0001" },
        new City { Name = "New York", Country = "USA", FolderPath = "nyc.jpg", PostOffice = "10001" },
        new City { Name = "Rome", Country = "Italy", FolderPath = "rome.jpg", PostOffice = "00100" },
        new City { Name = "Barcelona", Country = "Spain", FolderPath = "barcelona.jpg", PostOffice = "08001" }
    };

    public CityRepositoryTests()
    {
        var dbFactory = new DbContextFactory();
        _context = dbFactory.Create(DatabaseType.InMemory);
        _cityRepository = new CityRepository(_context);

    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnsPagedCities_WhenNoSearchTermProvided(int pageNumber, int pageSize)
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();

        var queryParameters = new CityQueryParameters { Page = pageNumber, PageSize = pageSize };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAllAsync(queryParameters);
        List<City> resultList = result.ToList();

        int expectedCount = Math.Max(0, Math.Min(pageSize, resultList.Count));
        // Assert
        resultList.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);

    }

    [Theory]
    [InlineData("Paris", 1, 10)]
    [InlineData("Rome", 2, 2)]
    [InlineData("UNKNOWN", 1, 10)]
    public async Task GetAll_WithSearchTerm_ShouldReturnFilteredCities(string searchTerm, int pageNumber, int pageSize)
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var queryParameters = new CityQueryParameters
        {
            Page = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAllAsync(queryParameters);
        List<City> resultList = result.ToList();

        var expectedResult = _cities.Where(c => c.Name.Contains(searchTerm) || c.Country.Contains(searchTerm)).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        // Assert
        resultList.Count.Should().Be(expectedResult.Count);
        resultList.Should().BeEquivalentTo(expectedResult);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }


    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefaults()
    {
        // Arrange


        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAllAsync(new CityQueryParameters());

        // Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnCity()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var cityId = _cities[0].Id;
        // Act
        var result = await _cityRepository.GetByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_cities.FirstOrDefault(c => c.Id == cityId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNull(int cityId)
    {
        // Act
        var result = await _cityRepository.GetByIdAsync(cityId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithValidCity_ShouldAddCityToContext()
    {
        // Arrange
        var city = new City { Name = "New City", Country = "New Country", FolderPath = "New City.jpg", PostOffice = "75000" };

        // Act
        var result = await _cityRepository.CreateAsync(city);
        await _cityRepository.SaveChangesAsync();
        // Assert
        result.Should().BeEquivalentTo(city);
        _context.Cities.Should().Contain(city);
    }


    [Fact]
    public async Task Delete_WithValidId_ShouldRemoveCityAndReturnIt()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var cityId = _cities[0].Id;
        // Act
        var deleteResult = await _cityRepository.DeleteAsync(cityId);
        var saveChangesResult = await _cityRepository.SaveChangesAsync();
        var getResult = await _cityRepository.GetByIdAsync(cityId);

        // Assert
        deleteResult.Should().NotBeNull();
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();

    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _cityRepository.DeleteAsync(-1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Name = "City1", Country = "Country1", FolderPath = "paris.jpg", PostOffice = "75000" },
            new City { Name = "City2", Country = "Country2", FolderPath = "paris.jpg", PostOffice = "75000" }
        };

        await _context.Cities.AddRangeAsync(cities);

        // Act
        var result = await _cityRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(cities.Count);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        // Act
        var result = await _cityRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}