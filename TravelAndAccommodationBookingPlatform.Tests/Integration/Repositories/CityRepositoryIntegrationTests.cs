using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Application.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class CityRepositoryIntegrationTests : IClassFixture<DatabaseFixture>, IDisposable
{
    private readonly SqlServerDbContext _context;
    private readonly CityRepository _cityRepository;

    private readonly List<City> _cities = new List<City>
    {
        new City { Name = "Paris", Country = "France", Thumbnail = "paris.jpg", PostOffice = "75000" },
        new City { Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", PostOffice = "100-0001" },
        new City { Name = "New York", Country = "USA", Thumbnail = "nyc.jpg", PostOffice = "10001" },
        new City { Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", PostOffice = "00100" },
        new City { Name = "Barcelona", Country = "Spain", Thumbnail = "barcelona.jpg", PostOffice = "08001" }
    };

    public CityRepositoryIntegrationTests(DatabaseFixture fixture)
    {
        _context = fixture.CreateContext();
        _cityRepository = new CityRepository(_context);
    }

    [Fact]
    public async Task GetAll_WithRealDatabase_ShouldReturnCorrectPaginationAndFilter()
    {
        // Arrange
        await CleanDatabase();
        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();

        var queryParameters = new CityQueryParameters
        {
            Page = 1,
            PageSize = 2,
            SearchTerm = "Paris"
        };

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        var actual = result.ToList();

        // Assert
        actual.Should().HaveCount(1);
        actual[0].Name.Should().Be("Paris");
        paginationMetaData.TotalCount.Should().Be(1);
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(2);
        paginationMetaData.TotalPageCount.Should().Be(1);
    }
    [Fact]
    public async Task CompleteWorkflow_CreateReadUpdateDelete_ShouldWorkCorrectly()
    {
        // Arrange
        await CleanDatabase();

        var newCity = new City
        {
            Name = "Workflow Test City",
            Country = "Workflow Test Country",
            Thumbnail = "paris.jpg",
            PostOffice = "75000"
        };

        // Act - Create
        var created = await _cityRepository.Create(newCity);
        await _cityRepository.SaveChangesAsync();

        // Assert - Create
        created.Should().NotBeNull();
        created.Id.Should().BeGreaterThan(0);
        created.Name.Should().Be("Workflow Test City");
        created.Country.Should().Be("Workflow Test Country");

        var cityId = created.Id;

        // Act - Read
        var fetched = await _cityRepository.GetById(cityId);

        // Assert - Read
        fetched.Should().NotBeNull();
        fetched!.Name.Should().Be("Workflow Test City");
        fetched.Country.Should().Be("Workflow Test Country");

        // Act - Update
        fetched.Name = "Updated Workflow City";
        fetched.Country = "Updated Country";
        var updated = await _cityRepository.UpdateAsync(fetched);
        await _cityRepository.SaveChangesAsync();

        // Assert - Update
        var verifyUpdate = await _cityRepository.GetById(cityId);
        verifyUpdate.Should().NotBeNull();
        verifyUpdate!.Name.Should().Be("Updated Workflow City");
        verifyUpdate.Country.Should().Be("Updated Country");

        // Act - Delete
        var deleted = await _cityRepository.Delete(cityId);
        await _cityRepository.SaveChangesAsync();

        // Assert - Delete
        deleted.Should().NotBeNull();
        deleted.Id.Should().Be(cityId);

        var verifyDelete = await _cityRepository.GetById(cityId);
        verifyDelete.Should().BeNull();
    }

    [Fact]
    public async Task GetAll_WithSearchAndPagination_ReturnsCorrectDataAndMetadata()
    {
        // Arrange
        await CleanDatabase();

        await _context.Cities.AddRangeAsync(_cities);
        await _context.SaveChangesAsync();

        var queryParameters = new CityQueryParameters
        {
            Page = 1,
            PageSize = 2,
            SearchTerm = "a"
        };

        //these outcomes are based on the list we created above
        const int expectedTotalMatches = 5;
        const int expectedPageCount = 3;

        // Act
        var (result, paginationMetaData) = await _cityRepository.GetAll(queryParameters);
        var actual = result.ToList();

        // Assert
        paginationMetaData.TotalPageCount.Should().Be(expectedPageCount);
        actual.All(c => c.Name.Contains("a") || c.Country.Contains("a")).Should().BeTrue();
        paginationMetaData.TotalCount.Should().Be(expectedTotalMatches);
    }

    [Fact]
    public async Task ConcurrentOperations_ShouldHandleCorrectly()
    {
        // Arrange
        await CleanDatabase();

        var tasks = new List<Task<City?>>();

        // Act - Create multiple cities concurrently
        for (int i = 0; i < 5; i++)
        {
            var cityIndex = i;
            tasks.Add(Task.Run(async () =>
            {
                var options = new DbContextOptionsBuilder<SqlServerDbContext>()
                    .UseInMemoryDatabase($"ConcurrentTest_{Guid.NewGuid()}")
                    .Options;

                using var context = new SqlServerDbContext(options);
                var repository = new CityRepository(context);

                var city = new City
                {
                    Name = $"Concurrent City {cityIndex}",
                    Country = $"Concurrent Country {cityIndex}",
                    Thumbnail = $"Concurrent Thumbnail.jpg {cityIndex}",
                    PostOffice = $"Concurrent PostOffice {cityIndex}",
                };

                var result = await repository.Create(city);
                await repository.SaveChangesAsync();
                return result;
            }));
        }

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().AllSatisfy(city => city.Should().NotBeNull());
        results.Select(c => c.Name).Should().OnlyHaveUniqueItems();
    }


    [Fact]
    public async Task SaveChanges_WithMultipleOperations_ShouldReturnCorrectCount()
    {
        // Arrange
        await CleanDatabase();

        var city1 = new City { Name = "Batch City 1", Country = "Batch Country", Thumbnail = "paris.jpg", PostOffice = "75000" };
        var city2 = new City { Name = "Batch City 2", Country = "Batch Country", Thumbnail = "Batch Thumbnail.jpg", PostOffice = "Batch postOffice" };

        // Act
        await _cityRepository.Create(city1);
        await _cityRepository.Create(city2);
        var saveResult = await _cityRepository.SaveChangesAsync();

        // Assert
        saveResult.Should().Be(2);

        var allCities = await _context.Cities
            .Where(c => c.Country == "Batch Country")
            .ToListAsync();
        allCities.Should().HaveCount(2);
    }


    private async Task CleanDatabase()
    {
        var existingCities = await _context.Cities.ToListAsync();
        _context.Cities.RemoveRange(existingCities);
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        CleanDatabase().Wait();
        _context.Dispose();
    }
}