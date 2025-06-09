using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class CityRepositoryTests : IDisposable
{
    private readonly SqlServerDbContext _context;
    private readonly CityRepository _cityRepository;

    readonly List<City> _cities = new List<City>
    {
        new City { Name = "Paris", Country = "France", Thumbnail = "paris.jpg", PostOffice = "75000" },
        new City { Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", PostOffice = "100-0001" },
        new City { Name = "New York", Country = "USA", Thumbnail = "nyc.jpg", PostOffice = "10001" },
        new City { Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", PostOffice = "00100" },
        new City { Name = "Barcelona", Country = "Spain", Thumbnail = "barcelona.jpg", PostOffice = "08001" }
    };

    public CityRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SqlServerDbContext(options);
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
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        List<City> resultList = result.ToList();

        int skip = (pageNumber - 1) * pageSize;
        int expectedCount = Math.Max(0, Math.Min(pageSize, _cities.Count - skip));
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
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        List<City> resultList = result.ToList();

        var expectedResult = _cities.Where(c => c.Name.Contains(searchTerm) || c.Country.Contains(searchTerm)).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        // Assert
        resultList.Count.Should().Be(expectedResult.Count);
        resultList.Should().BeEquivalentTo(expectedResult);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }


    [Fact]
    public async Task GetAll_WithNullParameters_ShouldUseDefaults()
    {
        // Arrange
        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(null);

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
        var result = await _cityRepository.GetById(cityId);

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
        var result = await _cityRepository.GetById(cityId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithValidCity_ShouldAddCityToContext()
    {
        // Arrange
        var city = new City { Name = "New City", Country = "New Country", Thumbnail = "New City.jpg", PostOffice = "75000" };

        // Act
        var result = await _cityRepository.Create(city);
        await _cityRepository.SaveChangesAsync();
        // Assert
        result.Should().BeEquivalentTo(city);
        _context.Cities.Should().Contain(city);
    }

    [Fact]
    public async Task Update_WithValidCity_ShouldUpdateCityInContext()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var updatedCity = new City { Id = 1, Name = "Updated Name", Country = "Updated Country", Thumbnail = "paris.jpg", PostOffice = "75000" };

        // Act
        var result = await _cityRepository.UpdateAsync(updatedCity);
        await _cityRepository.SaveChangesAsync();
        // Assert
        result.Should().BeEquivalentTo(updatedCity, options => options.Excluding(c => c.Id));
        _context.Entry(result!).State.Should().Be(EntityState.Unchanged);
    }
    [Fact]
    public async Task Update_WithInvalidCityId_ShouldReturnNull()
    {
        // Arrange

        var updatedCity = new City { Id = -1, Name = "Updated Name", Country = "Updated Country", Thumbnail = "paris.jpg", PostOffice = "75000" };

        // Act
        var result = await _cityRepository.UpdateAsync(updatedCity);
        await _cityRepository.SaveChangesAsync();
        // Assert
        result.Should().Be(null);
    }


    [Fact]
    public async Task Delete_WithValidId_ShouldRemoveCityAndReturnIt()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var cityId = _cities[0].Id;
        // Act
        var deleteResult = await _cityRepository.Delete(cityId);
        var saveChangesResult = await _cityRepository.SaveChangesAsync();
        var getResult = await _cityRepository.GetById(cityId);

        // Assert
        deleteResult.Should().NotBeNull();
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();

    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _cityRepository.Delete(-1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task SaveChanges_ShouldReturnNumberOfAffectedEntries()
    {
        // Arrange
        var cities = new List<City>
        {
            new City { Name = "City1", Country = "Country1", Thumbnail = "paris.jpg", PostOffice = "75000" },
            new City { Name = "City2", Country = "Country2", Thumbnail = "paris.jpg", PostOffice = "75000" }
        };

        await _context.Cities.AddRangeAsync(cities);

        // Act
        var result = await _cityRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(cities.Count);
    }

    [Fact]
    public async Task SaveChanges_WithNoChanges_ShouldReturnZero()
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