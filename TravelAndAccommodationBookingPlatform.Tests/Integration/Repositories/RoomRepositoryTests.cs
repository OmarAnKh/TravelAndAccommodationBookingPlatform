using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class RoomRepositoryTests : IDisposable
{

    private readonly IAppDbContext _context;
    private readonly IRoomRepository _roomRepository;

    public RoomRepositoryTests()
    {
        var dbContextFactory = new DbContextFactory();
        _context = dbContextFactory.Create(DatabaseType.InMemory);
        _roomRepository = new RoomRepository(_context);
    }

    private readonly List<Room> _rooms = new List<Room>
    {
        new Room { HotelId = 1, RoomType = RoomType.Single, Price = 120, Availability = Availability.Available },
        new Room { HotelId = 2, RoomType = RoomType.Deluxe, Price = 200, Availability = Availability.Unavailable },
        new Room { HotelId = 3, RoomType = RoomType.Suite, Price = 300, Availability = Availability.Available },
        new Room { HotelId = 4, RoomType = RoomType.Single, Price = 100, Availability = Availability.Unavailable },
        new Room { HotelId = 5, RoomType = RoomType.Deluxe, Price = 180, Availability = Availability.Available }
    };

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnPagedRooms(int pageNumber, int pageSize)
    {
        //Arrange
        await _context.Rooms.AddRangeAsync(_rooms);
        await _context.SaveChangesAsync();

        var queryParameters = new RoomQueryParameters()
        {
            Page = pageNumber,
            PageSize = pageSize
        };

        //Act
        var (entities, paginationMetaData) = await _roomRepository.GetAllAsync(queryParameters);
        var result = entities.ToList();

        int expectedCount = Math.Max(0, Math.Min(result.Count, pageSize));

        //Assert
        result.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Theory]
    [InlineData(1, 10, null, null, null, null)]
    [InlineData(1, 5, 1, null, null, null)]
    [InlineData(1, 3, null, RoomType.Deluxe, null, null)]
    [InlineData(1, 10, null, null, 100f, null)]
    [InlineData(1, 2, 2, RoomType.Suite, 150f, 2)]
    public async Task GetAll_WithQueryParameters_ShouldReturnFilteredRooms(
        int page,
        int pageSize,
        int? hotelId,
        RoomType? roomType,
        float? minPrice,
        int? adults)
    {
        // Arrange
        await _context.Rooms.AddRangeAsync(_rooms);
        await _context.SaveChangesAsync();

        var queryParams = new RoomQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            HotelId = hotelId,
            RoomType = roomType,
            MinPrice = minPrice,
            Adults = adults
        };

        // Act
        var (entities, paginationMetaData) = await _roomRepository.GetAllAsync(queryParams);
        var resultList = entities.ToList();

        // Expected query
        var expected = _rooms.AsQueryable();

        if (hotelId.HasValue)
            expected = expected.Where(r => r.HotelId == hotelId.Value);

        if (roomType.HasValue)
            expected = expected.Where(r => r.RoomType == roomType.Value);

        if (minPrice.HasValue)
            expected = expected.Where(r => r.Price >= minPrice.Value);

        if (adults.HasValue)
            expected = expected.Where(r => r.Adults >= adults.Value);

        var expectedPaged = expected
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Assert
        resultList.Count.Should().Be(expectedPaged.Count);
        resultList.Should().BeEquivalentTo(expectedPaged);
        paginationMetaData.CurrentPage.Should().Be(page);
        paginationMetaData.PageSize.Should().Be(pageSize);
    }

    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefaults()
    {
        //Arrange

        //Act
        var (entities, paginationMetaData) = await _roomRepository.GetAllAsync(new RoomQueryParameters());

        //Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnRoom()
    {
        //Arrange
        await _context.Rooms.AddRangeAsync(_rooms);
        await _context.SaveChangesAsync();

        //Act
        var result = await _roomRepository.GetByIdAsync(_rooms[0].Id);

        //Assert
        result.Should().BeEquivalentTo(_rooms[0]);

    }


    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound(int roomId)
    {
        //Arrange

        //Act 
        var result = await _roomRepository.GetByIdAsync(roomId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateRoom_WithValidParameters_ShouldCreateRoom()
    {
        //Arrange
        var room = new Room { HotelId = 2, RoomType = RoomType.Deluxe, Price = 200, Availability = Availability.Unavailable };

        //Act
        var createResult = await _roomRepository.CreateAsync(room);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        createResult.Should().BeEquivalentTo(room);
        saveChangesResult.Should().Be(1);

    }


    [Fact]
    public async Task DeleteRoom_WithValidParameters_ShouldDeleteRoom()
    {
        //Arrange
        await _context.Rooms.AddRangeAsync(_rooms);
        await _context.SaveChangesAsync();
        var room = await _roomRepository.GetByIdAsync(_rooms[0].Id);
        if (room == null)
        {
            Assert.Fail();
        }

        //Act
        var deleteResult = await _roomRepository.DeleteAsync(room.Id);
        var saveChangesResult = await _context.SaveChangesAsync();
        var getRoom = await _roomRepository.GetByIdAsync(room.Id);

        //Assert
        deleteResult.Should().BeEquivalentTo(room);
        saveChangesResult.Should().Be(1);
        getRoom.Should().BeNull();
    }

    [Fact]
    public async Task DeleteRoom_WithInvalidId_ShouldReturnNotFound()
    {
        //Arrange

        //Act
        var deleteResult = await _roomRepository.DeleteAsync(-1);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        //Arrange
        var rooms = new List<Room>
        {
            new Room { HotelId = 2, RoomType = RoomType.Deluxe, Price = 200, Availability = Availability.Unavailable },
            new Room { HotelId = 3, RoomType = RoomType.Suite, Price = 300, Availability = Availability.Available },
            new Room { HotelId = 4, RoomType = RoomType.Single, Price = 100, Availability = Availability.Unavailable },
        };

        //Act
        await _context.Rooms.AddRangeAsync(rooms);
        var saveChangesResult = await _roomRepository.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(rooms.Count);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        //Arrange

        //Act
        var saveChangesResult = await _roomRepository.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(0);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}