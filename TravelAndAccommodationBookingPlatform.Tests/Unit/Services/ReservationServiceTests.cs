// using AutoMapper;
// using FluentAssertions;
// using Moq;
// using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;
// using TravelAndAccommodationBookingPlatform.Application.Interfaces;
// using TravelAndAccommodationBookingPlatform.Application.Services;
// using TravelAndAccommodationBookingPlatform.Domain.Common;
// using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
// using TravelAndAccommodationBookingPlatform.Domain.Entities;
// using TravelAndAccommodationBookingPlatform.Domain.Enums;
// using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
// using Xunit;
//
// namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;
//
// public class ReservationServiceTests
// {
//     private readonly IReservationService _reservationService;
//     private readonly Mock<IReservationRepository> _reservationRepositoryMock;
//     private readonly Mock<IUserRepository> _userRepositoryMock;
//     private readonly Mock<IRoomRepository> _roomRepositoryMock;
//     private readonly Mock<IMapper> _mapperMock;
//
//     public ReservationServiceTests()
//     {
//         _reservationRepositoryMock = new Mock<IReservationRepository>();
//         _userRepositoryMock = new Mock<IUserRepository>();
//         _roomRepositoryMock = new Mock<IRoomRepository>();
//         _mapperMock = new Mock<IMapper>();
//
//         _reservationService = new ReservationService(
//             _reservationRepositoryMock.Object,
//             _roomRepositoryMock.Object,
//             _userRepositoryMock.Object,
//             _mapperMock.Object
//         );
//     }
//     [Fact]
//     public async Task GetAll_ShouldReturnMappedReservationsWithPagination()
//     {
//         // Arrange
//         const int userId = 1;
//         const int roomId = 2;
//         const float bookPrice = 150.0f;
//         var startDate = DateTime.Today.AddDays(1);
//         var endDate = startDate.AddDays(2);
//         var bookDate = DateTime.Today;
//         const int totalCount = 1;
//         const int pageNumber = 1;
//         const int pageSize = 10;
//         const int expectedCount = 1;
//
//         var queryParameters = new ReservationQueryParameters();
//         var reservations = new List<Reservation>()
//         {
//             new Reservation
//             {
//                 UserId = userId,
//                 RoomId = roomId,
//                 StartDate = startDate,
//                 EndDate = endDate,
//                 BookPrice = bookPrice,
//                 BookDate = bookDate,
//                 PaymentStatus = PaymentStatus.Pending,
//                 BookingStatus = BookingStatus.Pending
//             }
//         };
//         var paginationMetaData = new PaginationMetaData(totalCount, pageNumber, pageSize);
//
//         var reservationDto = new List<ReservationDto>()
//         {
//             new ReservationDto
//             {
//                 UserId = userId,
//                 RoomId = roomId,
//                 StartDate = startDate,
//                 EndDate = endDate,
//                 BookPrice = bookPrice,
//                 BookDate = bookDate,
//                 PaymentStatus = PaymentStatus.Pending,
//                 BookingStatus = BookingStatus.Pending
//             }
//         };
//
//         _reservationRepositoryMock.Setup(r => r.GetAllAsync(queryParameters))
//             .ReturnsAsync((reservations, paginationMetaData));
//         _mapperMock.Setup(m => m.Map<IEnumerable<ReservationDto>>(reservations))
//             .Returns(reservationDto);
//
//         // Act
//         var (result, pagination) = await _reservationService.GetAllAsync(queryParameters);
//         var resultList = result.ToList();
//
//         // Assert
//         resultList.Should().HaveCount(expectedCount);
//         resultList.Should().ContainSingle(r => r.UserId == userId && r.RoomId == roomId);
//         pagination.Should().BeEquivalentTo(paginationMetaData);
//     }
//
//     [Fact]
//     public async Task Create_ShouldReturnReservationDto_WhenDataIsValid()
//     {
//         // Arrange
//         var userId = 1;
//         var roomId = 10;
//         var startDate = DateTime.Today.AddDays(1);
//         var endDate = startDate.AddDays(2);
//
//         var creationDto = new ReservationCreationDto
//         {
//             UserId = userId,
//             RoomId = roomId,
//             StartDate = startDate,
//             EndDate = endDate,
//             BookPrice = 150.0f
//         };
//
//         var reservationEntity = new Reservation
//         {
//             UserId = userId,
//             RoomId = roomId,
//             StartDate = startDate,
//             EndDate = endDate,
//             BookPrice = 150.0f,
//             BookDate = creationDto.BookDate,
//             PaymentStatus = PaymentStatus.Pending,
//             BookingStatus = BookingStatus.Pending
//         };
//
//         var reservationDto = new ReservationDto
//         {
//             UserId = userId,
//             RoomId = roomId,
//             StartDate = startDate,
//             EndDate = endDate,
//             BookPrice = 150.0f,
//             BookDate = creationDto.BookDate,
//             PaymentStatus = PaymentStatus.Pending,
//             BookingStatus = BookingStatus.Pending
//         };
//
//         _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(new User { Id = userId });
//         _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId)).ReturnsAsync(new Room { Id = roomId });
//         _mapperMock.Setup(m => m.Map<Reservation>(creationDto)).Returns(reservationEntity);
//         _reservationRepositoryMock.Setup(r => r.CreateAsync(reservationEntity)).ReturnsAsync(reservationEntity);
//         _mapperMock.Setup(m => m.Map<ReservationDto>(reservationEntity)).Returns(reservationDto);
//
//         // Act
//         var result = await _reservationService.CreateAsync(creationDto);
//
//         // Assert
//         Assert.NotNull(result);
//         Assert.Equal(userId, result.UserId);
//         Assert.Equal(roomId, result.RoomId);
//         Assert.Equal(startDate, result.StartDate);
//         Assert.Equal(endDate, result.EndDate);
//     }
//
//     [Fact]
//     public async Task Create_ShouldReturnNull_WhenUserIsNotFound()
//     {
//         // Arrange
//         var creationDto = new ReservationCreationDto
//         {
//             UserId = 2,
//             RoomId = 20,
//             StartDate = DateTime.Today.AddDays(1),
//             EndDate = DateTime.Today.AddDays(2),
//             BookPrice = 100.0f
//         };
//
//         _userRepositoryMock.Setup(r => r.GetByIdAsync(creationDto.UserId)).ReturnsAsync((User?)null);
//
//         // Act
//         var result = await _reservationService.CreateAsync(creationDto);
//
//         // Assert
//         Assert.Null(result);
//     }
//
//     [Fact]
//     public async Task Create_ShouldReturnNull_WhenRoomIsNotFound()
//     {
//         // Arrange
//         var creationDto = new ReservationCreationDto
//         {
//             UserId = 3,
//             RoomId = 30,
//             StartDate = DateTime.Today.AddDays(1),
//             EndDate = DateTime.Today.AddDays(2),
//             BookPrice = 120.0f
//         };
//
//         _userRepositoryMock.Setup(r => r.GetByIdAsync(creationDto.UserId)).ReturnsAsync(new User { Id = creationDto.UserId });
//         _roomRepositoryMock.Setup(r => r.GetByIdAsync(creationDto.RoomId)).ReturnsAsync((Room?)null);
//
//         // Act
//         var result = await _reservationService.CreateAsync(creationDto);
//
//         // Assert
//         Assert.Null(result);
//     }
//
//     [Fact]
//     public async Task Create_ShouldReturnNull_WhenDateIsInvalid()
//     {
//         // Arrange
//         var creationDto = new ReservationCreationDto
//         {
//             UserId = 4,
//             RoomId = 40,
//             StartDate = DateTime.Today.AddDays(-1), // Invalid
//             EndDate = DateTime.Today,
//             BookPrice = 90.0f
//         };
//
//         // Act
//         var result = await _reservationService.CreateAsync(creationDto);
//
//         // Assert
//         Assert.Null(result);
//     }
//
//
//     [Theory]
//     [InlineData(1, 1)]
//     [InlineData(2, 2)]
//     public async Task GetUserIdAndRoomId_ShouldReturnDto_WhenRoomExists(int userId, int roomId)
//     {
//         //Arrange
//         var reservation = new Reservation()
//         {
//             UserId = userId,
//             RoomId = roomId,
//         };
//
//         _reservationRepositoryMock.Setup(r => r.GetByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync(reservation);
//         _mapperMock.Setup(m => m.Map<ReservationDto>(reservation)).Returns(new ReservationDto()
//         {
//             UserId = userId,
//             RoomId = roomId,
//         });
//
//         //Act
//         var result = await _reservationService.GetByUserAndRoomIdAsync(userId, roomId);
//
//         //Assert
//         result.Should().NotBeNull();
//         result.UserId.Should().Be(userId);
//         result.RoomId.Should().Be(roomId);
//
//     }
//     [Theory]
//     [InlineData(-1, -1)]
//     [InlineData(-1, null)]
//     [InlineData(null, -1)]
//     [InlineData(null, null)]
//     public async Task GetUserIdAndRoomId_ShouldReturnNull_WhenUserIsNotFound(int userId, int roomId)
//     {
//         //Arrange
//         _reservationRepositoryMock.Setup(r => r.GetByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync((Reservation?)null);
//
//         //Act
//         var result = await _reservationService.GetByUserAndRoomIdAsync(userId, roomId);
//
//         //Assert
//         result.Should().BeNull();
//     }
//
//
//     [Fact]
//     public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
//     {
//         // Arrange
//         const int userId = 1;
//         const int roomId = 5;
//         const PaymentStatus updatedPaymentStatus = PaymentStatus.Completed;
//         const BookingStatus updatedBookingStatus = BookingStatus.Confirmed;
//
//         var updateDto = new ReservationUpdateDto
//         {
//             UserId = userId,
//             RoomId = roomId,
//             PaymentStatus = PaymentStatus.Pending,
//             BookingStatus = BookingStatus.Pending
//         };
//
//         var reservation = new Reservation
//         {
//             UserId = userId,
//             RoomId = roomId,
//             PaymentStatus = PaymentStatus.Pending,
//             BookingStatus = BookingStatus.Pending
//         };
//
//         var updatedReservation = new Reservation
//         {
//             UserId = userId,
//             RoomId = roomId,
//             PaymentStatus = updatedPaymentStatus,
//             BookingStatus = updatedBookingStatus
//         };
//
//         _mapperMock.Setup(m => m.Map<Reservation>(updateDto)).Returns(reservation);
//         _reservationRepositoryMock.Setup(r => r.UpdateAsync(reservation)).ReturnsAsync(updatedReservation);
//         _mapperMock.Setup(m => m.Map<ReservationDto>(updatedReservation)).Returns(new ReservationDto
//         {
//             UserId = userId,
//             RoomId = roomId,
//             PaymentStatus = updatedPaymentStatus,
//             BookingStatus = updatedBookingStatus
//         });
//
//         // Act
//         var result = await _reservationService.UpdateAsync(updateDto);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Should().BeOfType<ReservationDto>();
//         result.UserId.Should().Be(userId);
//         result.RoomId.Should().Be(roomId);
//         result.PaymentStatus.Should().Be(updatedPaymentStatus);
//         result.BookingStatus.Should().Be(updatedBookingStatus);
//     }
//     [Fact]
//     public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
//     {
//         // Arrange
//         const int userId = 2;
//         const int roomId = 20;
//
//         var updateDto = new ReservationUpdateDto
//         {
//             UserId = userId,
//             RoomId = roomId
//         };
//
//         var reservation = new Reservation
//         {
//             UserId = userId,
//             RoomId = roomId
//         };
//
//         _mapperMock.Setup(m => m.Map<Reservation>(updateDto)).Returns(reservation);
//         _reservationRepositoryMock.Setup(r => r.UpdateAsync(reservation)).ReturnsAsync((Reservation?)null);
//
//         // Act
//         var result = await _reservationService.UpdateAsync(updateDto);
//
//         // Assert
//         result.Should().BeNull();
//     }
//
//     [Fact]
//     public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
//     {
//         // Arrange
//         const int userId = 3;
//         const int roomId = 4;
//
//         var reservation = new Reservation
//         {
//             UserId = userId,
//             RoomId = roomId
//         };
//
//         _reservationRepositoryMock.Setup(r => r.DeleteByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync(reservation);
//         _mapperMock.Setup(m => m.Map<ReservationDto>(reservation)).Returns(new ReservationDto
//         {
//             UserId = userId,
//             RoomId = roomId
//         });
//
//         // Act
//         var result = await _reservationService.DeleteByUserAndRoomIdAsync(userId, roomId);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.UserId.Should().Be(userId);
//     }
//
//     [Theory]
//     [InlineData(-1, -1)]
//     [InlineData(-1, null)]
//     [InlineData(null, -1)]
//     [InlineData(null, null)]
//     public async Task Delete_ShouldReturnNull_WhenDeletionFails(int userId, int roomId)
//     {
//         // Arrange
//         _reservationRepositoryMock.Setup(r => r.DeleteByUserAndRoomIdAsync(userId, roomId)).ReturnsAsync((Reservation?)null);
//
//         // Act
//         var result = await _reservationService.DeleteByUserAndRoomIdAsync(userId, roomId);
//
//         // Assert
//         result.Should().BeNull();
//     }
// }

