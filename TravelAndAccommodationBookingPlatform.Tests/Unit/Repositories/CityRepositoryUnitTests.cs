using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Repositories;

public class CityRepositoryUnitTests : IDisposable
{
    private readonly SqlServerDbContext _context;
    private readonly CityRepository _cityRepository;

    readonly List<City> _cities = new List<City>
    {
        new City { Id = 1, Name = "Paris", Country = "France", Thumbnail = "paris.jpg", PostOffice = "75000" },
        new City { Id = 2, Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", PostOffice = "100-0001" },
        new City { Id = 3, Name = "New York", Country = "USA", Thumbnail = "nyc.jpg", PostOffice = "10001" },
        new City { Id = 4, Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", PostOffice = "00100" },
        new City { Id = 5, Name = "Barcelona", Country = "Spain", Thumbnail = "barcelona.jpg", PostOffice = "08001" }
    };

    public CityRepositoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<SqlServerDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new SqlServerDbContext(options);
        _cityRepository = new CityRepository(_context);

    }

    [Fact]
    public async Task GetAll_WithoutSearchTerm_ShouldReturnAllCities()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        const int page = 1;
        const int pageSize = 10;
        var queryParameters = new CityQueryParameters { Page = page, PageSize = pageSize };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        IEnumerable<City> enumerable = result.ToList();

        // Assert
        enumerable.Should().HaveCount(_cities.Count);
        enumerable.Should().BeEquivalentTo(_cities);
        paginationMetaData.TotalCount.Should().Be(_cities.Count);
        paginationMetaData.CurrentPage.Should().Be(page);
        paginationMetaData.PageSize.Should().Be(pageSize);

    }

    [Fact]
    public async Task GetAll_WithSearchTerm_ShouldReturnFilteredCities()
    {
        // Arrange
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();
        var resultCollection = _cities.Where(c => c.Country.Contains(_cities.First().Country) || c.Name.Contains(_cities.First().Name)).ToList();
        var queryParameters = new CityQueryParameters
        {
            Page = 1,
            PageSize = 10,
            SearchTerm = _cities.First().Name
        };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        IEnumerable<City> enumerable = result.ToList();

        // Assert
        enumerable.Should().HaveCount(resultCollection.Count);
        enumerable.First().Name.Should().Be(resultCollection.First().Name);
        paginationMetaData.TotalCount.Should().Be(resultCollection.Count);
    }


    [Fact]
    public async Task GetAll_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        var listOfCities = new List<City>();
        for (int i = 1; i <= 15; i++)
        {
            listOfCities.Add(new City { Id = i, Name = $"City{i}", Country = $"Country{i}", Thumbnail = $"Thumbnail{i}", PostOffice = $"PostOffice{i}" });
        }

        await _context.Cities.AddRangeAsync(listOfCities);
        await _context.SaveChangesAsync();

        var page = 2;
        var pageSize = 5;
        var queryParameters = new CityQueryParameters { Page = page, PageSize = pageSize };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        IEnumerable<City> enumerable = result.ToList();

        // Assert
        enumerable.Should().HaveCount(pageSize);
        enumerable.First().Id.Should().Be(6); // Second page should start with 6th element
        paginationMetaData.TotalCount.Should().Be(15);
        paginationMetaData.CurrentPage.Should().Be(page);
        paginationMetaData.PageSize.Should().Be(pageSize);
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

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _cityRepository.GetById(999);

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

        var updatedCity = new City { Id = 999, Name = "Updated Name", Country = "Updated Country", Thumbnail = "paris.jpg", PostOffice = "75000" };

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
        var result = await _cityRepository.Delete(cityId);
        await _cityRepository.SaveChangesAsync();

        // Assert
        result.Should().BeEquivalentTo(_cities.FirstOrDefault(c => c.Id == cityId));
    }

    [Fact]
    public async Task Delete_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _cityRepository.Delete(999);

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