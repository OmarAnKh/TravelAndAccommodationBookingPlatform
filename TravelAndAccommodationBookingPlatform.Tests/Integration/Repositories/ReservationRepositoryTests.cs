using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common.DatabaseFactories;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class ReservationRepositoryTests : IDisposable
{
    private readonly IAppDbContext _context;
    private readonly IReservationRepository _reservationRepository;

    private List<Reservation> _reservations = new List<Reservation>
    {
        new Reservation
        {
            UserId = 1,
            RoomId = 101,
            StartDate = new DateTime(2025, 7, 1),
            EndDate = new DateTime(2025, 7, 5),
            BookPrice = 450.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Completed,
            BookingStatus = BookingStatus.Confirmed
        },
        new Reservation
        {
            UserId = 2,
            RoomId = 102,
            StartDate = new DateTime(2025, 8, 10),
            EndDate = new DateTime(2025, 8, 15),
            BookPrice = 600.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        },
        new Reservation
        {
            UserId = 3,
            RoomId = 103,
            StartDate = new DateTime(2025, 9, 20),
            EndDate = new DateTime(2025, 9, 25),
            BookPrice = 750.0f,
            BookDate = DateTime.UtcNow.AddDays(-1),
            PaymentStatus = PaymentStatus.Failed,
            BookingStatus = BookingStatus.Cancelled
        },
        new Reservation
        {
            UserId = 4,
            RoomId = 104,
            StartDate = new DateTime(2025, 10, 5),
            EndDate = new DateTime(2025, 10, 10),
            BookPrice = 500.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Completed,
            BookingStatus = BookingStatus.Confirmed
        },
        new Reservation
        {
            UserId = 5,
            RoomId = 105,
            StartDate = new DateTime(2025, 11, 1),
            EndDate = new DateTime(2025, 11, 3),
            BookPrice = 300.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Completed,
            BookingStatus = BookingStatus.Pending
        }
    };

    public ReservationRepositoryTests()
    {
        var dbFactory = new DbContextFactory();
        _context = dbFactory.Create(DatabaseType.InMemory);
        _reservationRepository = new ReservationRepository(_context);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnsPagedReservations_WhenNoSearchTermProvided(int pageNumber, int pageSize)
    {
        //Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();

        var queryParameter = new ReservationQueryParameters()
        {
            Page = pageNumber,
            PageSize = pageSize
        };

        //Act
        var (entities, paginationMetaData) = await _reservationRepository.GetAll(queryParameter);
        var resultList = entities.ToList();

        int expectedCount = Math.Max(0, Math.Min(resultList.Count, pageSize));

        //Assert
        resultList.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Theory]
    [InlineData(1, 10, null, null, null, null)]
    [InlineData(2, 5, "2025-06-01", "2025-06-10", 1, 1)]
    [InlineData(3, 20, "2025-01-01", null, 0, 0)]
    [InlineData(1, 15, null, "2025-12-31", 2, 2)]
    [InlineData(4, 50, "2025-08-01", "2025-08-05", null, 1)]
    public async Task GetAll_WithSearchTerm_ShouldReturnFilteredReservations(
        int page,
        int pageSize,
        string? startDateStr,
        string? endDateStr,
        int? paymentStatusInt,
        int? bookingStatusInt)
    {
        // Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();

        var queryParams = new ReservationQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            StartDate = string.IsNullOrEmpty(startDateStr) ? null : DateTime.Parse(startDateStr),
            EndDate = string.IsNullOrEmpty(endDateStr) ? null : DateTime.Parse(endDateStr),
            PaymentStatus = paymentStatusInt.HasValue ? (PaymentStatus?)paymentStatusInt : null,
            BookingStatus = bookingStatusInt.HasValue ? (BookingStatus?)bookingStatusInt : null
        };

        // Act
        var (entities, paginationMetaData) = await _reservationRepository.GetAll(queryParams);
        var resultList = entities.ToList();

        var expectedResult = _reservations.AsQueryable();

        if (queryParams.StartDate.HasValue)
            expectedResult = expectedResult.Where(r => r.StartDate >= queryParams.StartDate.Value);

        if (queryParams.EndDate.HasValue)
            expectedResult = expectedResult.Where(r => r.EndDate <= queryParams.EndDate.Value);

        if (queryParams.PaymentStatus.HasValue)
            expectedResult = expectedResult.Where(r => r.PaymentStatus == queryParams.PaymentStatus.Value);

        if (queryParams.BookingStatus.HasValue)
            expectedResult = expectedResult.Where(r => r.BookingStatus == queryParams.BookingStatus.Value);

        var expectedPaged = expectedResult
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Assert
        resultList.Count.Should().Be(expectedPaged.Count);
        resultList.Should().BeEquivalentTo(expectedPaged);
        paginationMetaData.CurrentPage.Should().Be(page);
    }

    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefault()
    {
        //Arrange

        //Act
        var (entities, paginationMetaData) = await _reservationRepository.GetAll(new ReservationQueryParameters());

        //Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnReservation()
    {
        //Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();

        //Act
        var result = await _reservationRepository.GetByUserAndRoomId(_reservations[0].UserId, _reservations[0].RoomId);

        //Assert
        result.Should().BeEquivalentTo(_reservations[0]);
    }

    [Theory]
    [InlineData(-1, -1)]
    [InlineData(null, null)]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound(int userId, int roomId)
    {
        //Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();

        //Act
        var result = await _reservationRepository.GetByUserAndRoomId(userId, roomId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateReservation_WithValidData_ShouldCreateReservation()
    {
        //Arrange
        var reservation = new Reservation
        {
            UserId = 2,
            RoomId = 102,
            StartDate = new DateTime(2025, 8, 10),
            EndDate = new DateTime(2025, 8, 15),
            BookPrice = 600.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        //Act
        var createResult = await _reservationRepository.Create(reservation);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        createResult.Should().Be(reservation);
        saveChangesResult.Should().Be(1);
    }

    [Fact]
    public async Task UpdateReservation_WithValidData_ShouldUpdateReservation()
    {
        //Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();
        var reservation = await _reservationRepository.GetByUserAndRoomId(_reservations[0].UserId, _reservations[0].RoomId);
        if (reservation == null)
        {
            Assert.Fail();
        }
        reservation.BookDate = DateTime.UtcNow;
        //Act
        var updateResult = await _reservationRepository.UpdateAsync(reservation);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        updateResult.Should().BeEquivalentTo(reservation);
        saveChangesResult.Should().Be(1);
    }

    [Fact]
    public async Task UpdateReservation_WithInvalidData_ShouldReturnNull()
    {
        //Arrange
        var reservation = new Reservation
        {
            UserId = -1,
            RoomId = -1,
            StartDate = new DateTime(2025, 8, 10),
            EndDate = new DateTime(2025, 8, 15),
            BookPrice = 600.0f,
            BookDate = DateTime.UtcNow,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };
        //Act
        var updateResult = await _reservationRepository.UpdateAsync(reservation);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        updateResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }

    [Fact]
    public async Task DeleteReservation_WithValidData_ShouldDeleteReservation()
    {
        //Arrange
        await _context.Reservations.AddRangeAsync(_reservations);
        await _context.SaveChangesAsync();
        var reservation = await _reservationRepository.GetByUserAndRoomId(_reservations[0].UserId, _reservations[0].RoomId);
        if (reservation == null)
        {
            Assert.Fail();
        }
        //Act
        var deleteResult = await _reservationRepository.DeleteByUserAndRoomId(reservation.UserId, reservation.RoomId);
        var saveChangesResult = await _context.SaveChangesAsync();
        var getResult = await _reservationRepository.GetByUserAndRoomId(reservation.UserId, reservation.RoomId);

        //Assert
        deleteResult.Should().BeEquivalentTo(reservation);
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();

    }


    [Fact]
    public async Task DeleteReservation_WithInvalidId_ShouldReturnNull()
    {
        //Arrange

        //Act
        var deleteResult = await _reservationRepository.DeleteByUserAndRoomId(-1, -1);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }


    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        //Arrange
        var reservations = new List<Reservation>
        {
            new Reservation
            {
                UserId = 1,
                RoomId = 101,
                StartDate = new DateTime(2025, 7, 1),
                EndDate = new DateTime(2025, 7, 5),
                BookPrice = 450.0f,
                BookDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Completed,
                BookingStatus = BookingStatus.Confirmed
            },
            new Reservation
            {
                UserId = 2,
                RoomId = 102,
                StartDate = new DateTime(2025, 8, 10),
                EndDate = new DateTime(2025, 8, 15),
                BookPrice = 600.0f,
                BookDate = DateTime.UtcNow,
                PaymentStatus = PaymentStatus.Pending,
                BookingStatus = BookingStatus.Pending
            }
        };
        await _context.Reservations.AddRangeAsync(reservations);
        //Act
        var result = await _context.SaveChangesAsync();

        //Assert
        result.Should().Be(reservations.Count);
    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        //Arrange

        //Act
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(0);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}