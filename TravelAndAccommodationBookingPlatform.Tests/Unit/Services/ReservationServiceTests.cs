using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class ReservationServiceTests
{
    private readonly IReservationService _reservationService;
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public ReservationServiceTests()
    {
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _mapperMock = new Mock<IMapper>();

        _reservationService = new ReservationService(
            _reservationRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapperMock.Object
        );
    }
    [Fact]
    public async Task GetAll_ShouldReturnMappedReservationsWithPagination()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 2;
        const float bookPrice = 150.0f;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = startDate.AddDays(2);
        var bookDate = DateTime.Today;
        const int totalCount = 1;
        const int pageNumber = 1;
        const int pageSize = 10;
        const int expectedCount = 1;

        var queryParameters = new ReservationQueryParameters();
        var reservations = new List<Reservation>()
        {
            new Reservation
            {
                UserId = userId,
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate,
                BookPrice = bookPrice,
                BookDate = bookDate,
                PaymentStatus = PaymentStatus.Pending,
                BookingStatus = BookingStatus.Pending
            }
        };
        var paginationMetaData = new PaginationMetaData(totalCount, pageNumber, pageSize);

        var reservationDto = new List<ReservationDto>()
        {
            new ReservationDto
            {
                UserId = userId,
                RoomId = roomId,
                StartDate = startDate,
                EndDate = endDate,
                BookPrice = bookPrice,
                BookDate = bookDate,
                PaymentStatus = PaymentStatus.Pending,
                BookingStatus = BookingStatus.Pending
            }
        };

        _reservationRepositoryMock.Setup(r => r.GetAllAsync(queryParameters))
            .ReturnsAsync((reservations, paginationMetaData));
        _mapperMock.Setup(m => m.Map<IEnumerable<ReservationDto>>(reservations))
            .Returns(reservationDto);

        // Act
        var (result, pagination) = await _reservationService.GetAllAsync(queryParameters);
        var resultList = result.ToList();

        // Assert
        resultList.Should().HaveCount(expectedCount);
        resultList.Should().ContainSingle(r => r.UserId == userId && r.RoomId == roomId);
        pagination.Should().BeEquivalentTo(paginationMetaData);
    }

    [Fact]
    public async Task Create_ShouldReturnReservationDto_WhenDataIsValid()
    {
        // Arrange
        var userId = 1;
        var roomId = 10;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = startDate.AddDays(2);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        var reservationEntity = new Reservation
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f,
            BookDate = DateTime.Now,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        var reservationDto = new ReservationDto
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f,
            BookDate = DateTime.Now,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId)).ReturnsAsync(new Room { Id = roomId });
        _mapperMock.Setup(m => m.Map<Reservation>(creationDto)).Returns(reservationEntity);
        _reservationRepositoryMock.Setup(r => r.CreateAsync(reservationEntity)).ReturnsAsync(reservationEntity);
        _mapperMock.Setup(m => m.Map<ReservationDto>(reservationEntity)).Returns(reservationDto);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.RoomId.Should().Be(roomId);
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(endDate);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenUserIsNotFound()
    {
        // Arrange
        int userId = 2;
        var creationDto = new ReservationCreationDto
        {
            RoomId = 20,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            BookPrice = 100.0f
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRoomIsNotFound()
    {
        // Arrange
        int userId = 3;
        var creationDto = new ReservationCreationDto
        {
            RoomId = 30,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(2),
            BookPrice = 120.0f
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(creationDto.RoomId)).ReturnsAsync((Room?)null);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenDateIsInvalid()
    {
        // Arrange
        int userId = 4;
        var creationDto = new ReservationCreationDto
        {
            RoomId = 40,
            StartDate = DateTime.Today.AddDays(-1),
            EndDate = DateTime.Today,
            BookPrice = 90.0f
        };

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldSetCorrectDefaultValues_WhenCreatingReservation()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(3);
        var beforeTest = DateTime.Now.AddSeconds(-1);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        var reservationEntity = new Reservation();
        var createdReservation = new Reservation
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f,
            BookDate = DateTime.Now,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        var reservationDto = new ReservationDto
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f,
            BookDate = DateTime.Now,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId))
            .ReturnsAsync(new Room { Id = roomId });
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingAsync(userId, roomId, startDate, endDate))
            .ReturnsAsync(false);
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingForRoomAsync(roomId, startDate, endDate))
            .ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Reservation>(creationDto))
            .Returns(reservationEntity);
        _reservationRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
            .ReturnsAsync(createdReservation);
        _mapperMock.Setup(m => m.Map<ReservationDto>(createdReservation))
            .Returns(reservationDto);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().NotBeNull();
        _reservationRepositoryMock.Verify(r => r.CreateAsync(It.Is<Reservation>(res =>
            res.UserId == userId &&
            res.BookDate >= beforeTest &&
            res.BookDate <= DateTime.Now.AddSeconds(1))), Times.Once);
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldValidateStartDateIsToday_WhenStartDateIsToday()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(1);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        var reservationEntity = new Reservation();
        var createdReservation = new Reservation
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        var reservationDto = new ReservationDto
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId))
            .ReturnsAsync(new Room { Id = roomId });
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingAsync(userId, roomId, startDate, endDate))
            .ReturnsAsync(false);
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingForRoomAsync(roomId, startDate, endDate))
            .ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Reservation>(creationDto))
            .Returns(reservationEntity);
        _reservationRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Reservation>()))
            .ReturnsAsync(createdReservation);
        _mapperMock.Setup(m => m.Map<ReservationDto>(createdReservation))
            .Returns(reservationDto);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.StartDate.Should().Be(startDate);
        result.EndDate.Should().Be(endDate);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenDateRangeOverlapsWithUserReservation()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(3);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId))
            .ReturnsAsync(new Room { Id = roomId });
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingAsync(userId, roomId, startDate, endDate))
            .ReturnsAsync(true);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRoomIsAlreadyBooked()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(3);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId))
            .ReturnsAsync(new Room { Id = roomId });
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingAsync(userId, roomId, startDate, endDate))
            .ReturnsAsync(false);
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingForRoomAsync(roomId, startDate, endDate))
            .ReturnsAsync(true);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryCreateFails()
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today.AddDays(1);
        var endDate = DateTime.Today.AddDays(3);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        var reservationEntity = new Reservation
        {
            UserId = userId,
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f,
            BookDate = DateTime.Now
        };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId))
            .ReturnsAsync(new Room { Id = roomId });
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingAsync(userId, roomId, startDate, endDate))
            .ReturnsAsync(false);
        _reservationRepositoryMock.Setup(r => r.IsDateRangeOverlappingForRoomAsync(roomId, startDate, endDate))
            .ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<Reservation>(creationDto))
            .Returns(reservationEntity);
        _reservationRepositoryMock.Setup(r => r.CreateAsync(reservationEntity))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(-2, -1)]
    [InlineData(0, -1)]
    [InlineData(1, 0)]
    public async Task Create_ShouldReturnNull_WhenDatesAreInvalid(int startDaysFromToday, int endDaysFromToday)
    {
        // Arrange
        const int userId = 1;
        const int roomId = 1;
        var startDate = DateTime.Today.AddDays(startDaysFromToday);
        var endDate = DateTime.Today.AddDays(endDaysFromToday);

        var creationDto = new ReservationCreationDto
        {
            RoomId = roomId,
            StartDate = startDate,
            EndDate = endDate,
            BookPrice = 150.0f
        };

        // Act
        var result = await _reservationService.CreateAsync(creationDto, userId);

        // Assert
        result.Should().BeNull();
    }
    
    [Theory]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    public async Task GetUserIdAndRoomId_ShouldReturnDto_WhenReservationExists(int userId, int roomId)
    {
        //Arrange
        var reservation = new Reservation()
        {
            UserId = userId,
            RoomId = roomId,
        };

        _reservationRepositoryMock.Setup(r => r.GetByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync(reservation);
        _mapperMock.Setup(m => m.Map<ReservationDto>(reservation)).Returns(new ReservationDto()
        {
            UserId = userId,
            RoomId = roomId,
        });

        //Act
        var result = await _reservationService.GetByUserAndRoomIdAsync(userId, roomId);

        //Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(userId);
        result.RoomId.Should().Be(roomId);

    }

    [Theory]
    [InlineData(-1, -1)]
    [InlineData(-1, null)]
    [InlineData(null, -1)]
    [InlineData(null, null)]
    public async Task GetUserIdAndRoomId_ShouldReturnNull_WhenNotFound(int userId, int roomId)
    {
        //Arrange
        _reservationRepositoryMock.Setup(r => r.GetByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync((Reservation?)null);

        //Act
        var result = await _reservationService.GetByUserAndRoomIdAsync(userId, roomId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldApplyPatchCorrectly_WhenUpdatingMultipleFields()
    {
        // Arrange
        const int reservationId = 1;
        var existingReservation = new Reservation
        {
            Id = reservationId,
            UserId = 1,
            RoomId = 5,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending,
            BookPrice = 100.0f
        };

        var patchDoc = new JsonPatchDocument<ReservationUpdateDto>();
        patchDoc.Replace(r => r.PaymentStatus, PaymentStatus.Completed);
        patchDoc.Replace(r => r.BookingStatus, BookingStatus.Confirmed);

        var updateDto = new ReservationUpdateDto
        {
            UserId = 1,
            RoomId = 5,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending,
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(existingReservation);
        _mapperMock.Setup(m => m.Map<ReservationUpdateDto>(existingReservation))
            .Returns(updateDto);
        _mapperMock.Setup(m => m.Map(It.IsAny<ReservationUpdateDto>(), existingReservation))
            .Callback<ReservationUpdateDto, Reservation>((dto, res) =>
            {
                res.PaymentStatus = dto.PaymentStatus;
                res.BookingStatus = dto.BookingStatus;
            });
        _mapperMock.Setup(m => m.Map<ReservationDto>(existingReservation))
            .Returns(new ReservationDto
            {
                Id = reservationId,
                UserId = 1,
                RoomId = 5,
                PaymentStatus = PaymentStatus.Completed,
                BookingStatus = BookingStatus.Confirmed,
                BookPrice = 200.0f
            });

        // Act
        var result = await _reservationService.UpdateAsync(reservationId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.PaymentStatus.Should().Be(PaymentStatus.Completed);
        result.BookingStatus.Should().Be(BookingStatus.Confirmed);
        result.BookPrice.Should().Be(200.0f);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        // Arrange
        const int reservationId = 1;
        const int userId = 1;
        const int roomId = 5;

        var existingReservation = new Reservation
        {
            Id = reservationId,
            UserId = userId,
            RoomId = roomId,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        var patchDoc = new JsonPatchDocument<ReservationUpdateDto>();
        patchDoc.Replace(r => r.PaymentStatus, PaymentStatus.Completed);
        patchDoc.Replace(r => r.BookingStatus, BookingStatus.Confirmed);

        _reservationRepositoryMock
            .Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(existingReservation);

        _mapperMock
            .Setup(m => m.Map<ReservationUpdateDto>(existingReservation))
            .Returns(new ReservationUpdateDto
            {
                UserId = userId,
                RoomId = roomId,
                PaymentStatus = PaymentStatus.Pending,
                BookingStatus = BookingStatus.Pending
            });

        _mapperMock
            .Setup(m => m.Map(It.IsAny<ReservationUpdateDto>(), existingReservation))
            .Callback<ReservationUpdateDto, Reservation>((dto, res) =>
            {
                res.PaymentStatus = dto.PaymentStatus;
                res.BookingStatus = dto.BookingStatus;
            });

        _mapperMock
            .Setup(m => m.Map<ReservationDto>(existingReservation))
            .Returns(new ReservationDto
            {
                UserId = userId,
                RoomId = roomId,
                PaymentStatus = PaymentStatus.Completed,
                BookingStatus = BookingStatus.Confirmed
            });

        // Act
        var result = await _reservationService.UpdateAsync(reservationId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ReservationDto>();
        result.UserId.Should().Be(userId);
        result.RoomId.Should().Be(roomId);
        result.PaymentStatus.Should().Be(PaymentStatus.Completed);
        result.BookingStatus.Should().Be(BookingStatus.Confirmed);

        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenReservationNotFound()
    {
        // Arrange
        const int reservationId = 99;

        var patchDoc = new JsonPatchDocument<ReservationUpdateDto>();
        patchDoc.Replace(r => r.PaymentStatus, PaymentStatus.Completed);

        _reservationRepositoryMock
            .Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.UpdateAsync(reservationId, patchDoc);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        int reservationId = 1;

        var reservation = new Reservation
        {
            Id = reservationId
        };

        _reservationRepositoryMock.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync(reservation);
        _mapperMock.Setup(m => m.Map<ReservationDto>(reservation)).Returns(new ReservationDto
        {
            Id = reservationId
        });

        // Act
        var result = await _reservationService.DeleteAsync(reservationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reservationId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails(int reservationId)
    {
        // Arrange
        _reservationRepositoryMock.Setup(r => r.DeleteAsync(reservationId)).ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.DeleteAsync(reservationId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnReservationDto_WhenReservationExists()
    {
        // Arrange
        const int reservationId = 1;
        var reservation = new Reservation
        {
            Id = reservationId,
            UserId = 1,
            RoomId = 1,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(3),
            BookPrice = 200.0f,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        var reservationDto = new ReservationDto
        {
            Id = reservationId,
            UserId = 1,
            RoomId = 1,
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(3),
            BookPrice = 200.0f,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(reservation);
        _mapperMock.Setup(m => m.Map<ReservationDto>(reservation))
            .Returns(reservationDto);

        // Act
        var result = await _reservationService.GetByIdAsync(reservationId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(reservationId);
        result.UserId.Should().Be(reservation.UserId);
        result.RoomId.Should().Be(reservation.RoomId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenReservationDoesNotExist()
    {
        // Arrange
        const int reservationId = 99;

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.GetByIdAsync(reservationId);

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task MarkAsPaidAsync_ShouldReturnTrue_WhenReservationExists()
    {
        // Arrange
        const int reservationId = 1;
        var reservation = new Reservation
        {
            Id = reservationId,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(reservation);

        // Act
        var result = await _reservationService.MarkAsPaidAsync(reservationId);

        // Assert
        result.Should().BeTrue();
        reservation.PaymentStatus.Should().Be(PaymentStatus.Completed);
        reservation.BookingStatus.Should().Be(BookingStatus.Confirmed);
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkAsPaidAsync_ShouldReturnFalse_WhenReservationDoesNotExist()
    {
        // Arrange
        const int reservationId = 99;

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.MarkAsPaidAsync(reservationId);

        // Assert
        result.Should().BeFalse();
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Fact]
    public async Task MarkAsFailedAsync_ShouldReturnTrue_WhenReservationExists()
    {
        // Arrange
        const int reservationId = 1;
        var reservation = new Reservation
        {
            Id = reservationId,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(reservation);

        // Act
        var result = await _reservationService.MarkAsFailedAsync(reservationId);

        // Assert
        result.Should().BeTrue();
        reservation.PaymentStatus.Should().Be(PaymentStatus.Failed);
        reservation.BookingStatus.Should().Be(BookingStatus.Cancelled);
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkAsFailedAsync_ShouldReturnFalse_WhenReservationDoesNotExist()
    {
        // Arrange
        const int reservationId = 99;

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.MarkAsFailedAsync(reservationId);

        // Assert
        result.Should().BeFalse();
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


    [Fact]
    public async Task MarkAsCancelledAsync_ShouldReturnTrue_WhenReservationExists()
    {
        // Arrange
        const int reservationId = 1;
        var reservation = new Reservation
        {
            Id = reservationId,
            PaymentStatus = PaymentStatus.Pending,
            BookingStatus = BookingStatus.Pending
        };

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync(reservation);

        // Act
        var result = await _reservationService.MarkAsCancelledAsync(reservationId);

        // Assert
        result.Should().BeTrue();
        reservation.PaymentStatus.Should().Be(PaymentStatus.Canceled);
        reservation.BookingStatus.Should().Be(BookingStatus.Cancelled);
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task MarkAsCancelledAsync_ShouldReturnFalse_WhenReservationDoesNotExist()
    {
        // Arrange
        const int reservationId = 99;

        _reservationRepositoryMock.Setup(r => r.GetByIdAsync(reservationId))
            .ReturnsAsync((Reservation?)null);

        // Act
        var result = await _reservationService.MarkAsCancelledAsync(reservationId);

        // Assert
        result.Should().BeFalse();
        _reservationRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


}