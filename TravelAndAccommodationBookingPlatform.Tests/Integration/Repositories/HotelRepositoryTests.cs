using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class HotelRepositoryTests : IDisposable
{
    private readonly IHotelRepository _hotelRepository;
    private readonly IAppDbContext _context;

    readonly List<Hotel> _hotels = new List<Hotel>()
    {
        new Hotel { Name = "Eiffel Hotel", CityId = 1, Owner = "Anan Khalili", Description = "Near Eiffel Tower", Thumbnail = "eiffel_hotel.jpg" },
        new Hotel { Name = "Shibuya Inn", CityId = 2, Owner = "Idk", Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg" },
        new Hotel { Name = "Times Square Hotel", CityId = 3, Owner = "Ahmad", Description = "Close to Broadway", Thumbnail = "ts_hotel.jpg" },
        new Hotel { Name = "Colosseum Suites", CityId = 4, Owner = "Rahaf", Description = "View of the Colosseum", Thumbnail = "colosseum.jpg" },
        new Hotel { Name = "Sagrada Familia Hotel", CityId = 5, Owner = "YOU", Description = "Near Gaudi's masterpiece", Thumbnail = "sagrada.jpg" }
    };

    public HotelRepositoryTests()
    {
        var dbFactory = new DbContextFactory();
        _context = dbFactory.Create(DatabaseType.InMemory);
        _hotelRepository = new HotelRepository(_context);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnsPagedHotels_WhenNoSearchTermProvided(int pageNumber, int pageSize)
    {
        // Arrange
        await _context.Hotels.AddRangeAsync(_hotels);
        await _context.SaveChangesAsync();

        var queryParameters = new HotelQueryParameters
        {
            Page = pageNumber,
            PageSize = pageSize,
        };

        // Act
        var (entities, paginationMetaData) = await _hotelRepository.GetAllAsync(queryParameters);
        var resultList = entities.ToList();

        int expectedCount = Math.Max(0, Math.Min(pageSize, resultList.Count));

        // Assert
        resultList.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Theory]
    [InlineData("Eiffel Hotel", 1, 10)]
    [InlineData("Shibuya Inn", 2, 2)]
    [InlineData("UNKNOWN", 1, 10)]
    public async Task GetAll_WithSearchTerm_ShouldReturnFilteredCities(string searchTerm, int pageNumber, int pageSize)
    {
        //Arrange
        await _context.Hotels.AddRangeAsync(_hotels);
        await _context.SaveChangesAsync();
        var queryParameters = new HotelQueryParameters
        {
            Page = pageNumber,
            PageSize = pageSize,
            SearchTerm = searchTerm
        };

        //Act
        var (entities, paginationMetaData) = await _hotelRepository.GetAllAsync(queryParameters);
        var resultList = entities.ToList();

        var expectedResult = _hotels.Where(hotel => hotel.Name.Contains(searchTerm)
                                                    || hotel.Description.Contains(searchTerm)).Skip(pageSize * (pageNumber - 1)).Take(pageSize).ToList();

        //Assert
        resultList.Count.Should().Be(expectedResult.Count);
        resultList.Should().BeEquivalentTo(expectedResult);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefaults()
    {
        // Arrange
        // Act
        var (result, paginationMetaData) = await _hotelRepository.GetAllAsync(new HotelQueryParameters());

        // Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnHotel()
    {
        // Arrange
        await _context.Hotels.AddRangeAsync(_hotels);
        await _context.SaveChangesAsync();
        var cityId = _hotels[0].Id;
        // Act
        var result = await _hotelRepository.GetByIdAsync(cityId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(_hotels.FirstOrDefault(c => c.Id == cityId));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNull(int hotelId)
    {
        // Arrange
        await _context.Hotels.AddRangeAsync(_hotels);
        await _context.SaveChangesAsync();

        // Act
        var result = await _hotelRepository.GetByIdAsync(hotelId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_WithValidHotel_ShouldAddCityToContext()
    {
        //Arrange

        var hotel = new Hotel { Name = "Shibuya Inn", CityId = 2, Owner = "Idk", Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg" };

        //Act
        var result = await _hotelRepository.CreateAsync(hotel);
        await _hotelRepository.SaveChangesAsync();

        //Assert
        result.Should().BeEquivalentTo(hotel);
        _context.Hotels.Should().Contain(hotel);

    }


    [Fact]
    public async Task Delete_WithValidHotel_ShouldDeleteCityInContext()
    {
        //Arrange
        await _context.Hotels.AddRangeAsync(_hotels);
        await _context.SaveChangesAsync();
        var hotel = await _hotelRepository.GetByIdAsync(_hotels[0].Id);

        //Act
        var result = await _hotelRepository.DeleteAsync(hotel.Id);
        var saveChangesResult = await _hotelRepository.SaveChangesAsync();
        var getResult = await _hotelRepository.GetByIdAsync(hotel.Id);

        //Assert
        result.Should().NotBeNull();
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();

    }

    [Fact]
    public async Task Delete_WithInvalidHotel_ShouldReturnNull()
    {
        //Arrange
        var hotel = new Hotel { Id = -1, Name = "Shibuya Inn", CityId = 2, Owner = "Idk", Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg" };

        //Act
        var deleteResult = await _hotelRepository.DeleteAsync(hotel.Id);
        var saveChangesResult = await _hotelRepository.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }


    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        // Arrange
        var hotels = new List<Hotel>
        {
            new Hotel { Name = "Shibuya Inn", CityId = 2, Owner = "Idk", Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg" },
            new Hotel { Name = "Times Square Hotel", CityId = 3, Owner = "Ahmad", Description = "Close to Broadway", Thumbnail = "ts_hotel.jpg" }
        };

        await _context.Hotels.AddRangeAsync(hotels);

        // Act
        var result = await _hotelRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(hotels.Count);
    }


    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        // Act
        var result = await _hotelRepository.SaveChangesAsync();

        // Assert
        result.Should().Be(0);
    }

    public void Dispose()
    {
        _context.Dispose();

    }
}