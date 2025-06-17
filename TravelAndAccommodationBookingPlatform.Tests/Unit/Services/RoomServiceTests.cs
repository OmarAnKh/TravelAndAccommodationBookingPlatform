// using AutoMapper;
// using FluentAssertions;
// using Moq;
// using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
// using TravelAndAccommodationBookingPlatform.Application.Interfaces;
// using TravelAndAccommodationBookingPlatform.Application.Services;
// using TravelAndAccommodationBookingPlatform.Domain.Common;
// using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
// using TravelAndAccommodationBookingPlatform.Domain.Entities;
// using TravelAndAccommodationBookingPlatform.Domain.Enums;
// using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
//
// namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;
//
// public class RoomServiceTests
// {
//     private readonly IRoomService _roomService;
//     private readonly Mock<IRoomRepository> _roomRepositoryMock;
//     private readonly Mock<IHotelRepository> _hotelRepositoryMock;
//     private readonly Mock<IMapper> _mapperMock;
//     public RoomServiceTests()
//     {
//         _roomRepositoryMock = new Mock<IRoomRepository>();
//         _hotelRepositoryMock = new Mock<IHotelRepository>();
//         _mapperMock = new Mock<IMapper>();
//         _roomService = new RoomService(_roomRepositoryMock.Object, _hotelRepositoryMock.Object, _mapperMock.Object);
//     }
//
//     [Fact]
//     public async Task GetAll_ShouldReturnMappedRoomsAndPagination()
//     {
//         // Arrange
//         var queryParams = new RoomQueryParameters();
//         var rooms = new List<Room>
//         {
//             new Room { Id = 1, HotelId = 1, RoomType = RoomType.Deluxe, Price = 100 },
//             new Room { Id = 2, HotelId = 1, RoomType = RoomType.Single, Price = 70 }
//         };
//         var pagination = new PaginationMetaData(2, 1, 10);
//
//         _roomRepositoryMock.Setup(r => r.GetAllAsync(queryParams))
//             .ReturnsAsync((rooms, pagination));
//
//         _mapperMock.Setup(m => m.Map<IEnumerable<RoomDto>>(rooms)).Returns(new List<RoomDto>
//         {
//             new RoomDto { Id = 1, HotelId = 1, RoomType = RoomType.Deluxe, Price = 100 },
//             new RoomDto { Id = 2, HotelId = 1, RoomType = RoomType.Single, Price = 70 }
//         });
//
//         // Act
//         var (result, meta) = await _roomService.GetAllAsync(queryParams);
//         var resultList = result.ToList();
//
//         // Assert
//         resultList.Count.Should().Be(2);
//         meta.TotalCount.Should().Be(2);
//     }
//
//     [Fact]
//     public async Task Create_ShouldReturnRoomDto_WhenSuccessful()
//     {
//         // Arrange
//         var creationDto = new RoomCreationDto
//         {
//             HotelId = 1,
//             CustomRoomTypeName = "Suite",
//             Price = 150
//         };
//
//         var hotel = new Hotel { Id = 1 };
//
//         var room = new Room
//         {
//             Id = 5,
//             HotelId = 1,
//             CustomRoomTypeName = "Suite",
//             Price = 150,
//             CreatedAt = DateTime.UtcNow,
//             UpdatedAt = DateTime.UtcNow
//         };
//
//         var expectedDto = new RoomDto
//         {
//             Id = 5,
//             HotelId = 1,
//             CustomRoomTypeName = "Suite",
//             Price = 150,
//             CreatedAt = room.CreatedAt,
//             UpdatedAt = room.UpdatedAt
//         };
//
//         _hotelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);
//         _mapperMock.Setup(m => m.Map<Room>(creationDto)).Returns(room);
//         _roomRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Room>())).ReturnsAsync(room);
//         _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(expectedDto);
//
//         // Act
//         var result = await _roomService.CreateAsync(creationDto);
//
//         // Assert
//         result.Should().NotBeNull();
//         result.Id.Should().Be(5);
//         result.HotelId.Should().Be(1);
//         result.CustomRoomTypeName.Should().Be("Suite");
//         result.Price.Should().Be(150);
//     }
//     [Fact]
//     public async Task Create_ShouldReturnNull_WhenHotelDoesNotExist()
//     {
//         // Arrange
//         var creationDto = new RoomCreationDto
//         {
//             HotelId = 4,
//             RoomType = RoomType.Apartment,
//             Price = 543
//         };
//
//         _hotelRepositoryMock.Setup(r => r.GetByIdAsync(-1)).ReturnsAsync((Hotel?)null);
//
//         // Act
//         var result = await _roomService.CreateAsync(creationDto);
//
//         // Assert
//         result.Should().BeNull();
//     }
//
//
//     [Fact]
//     public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
//     {
//         // Arrange
//         var creationDto = new RoomCreationDto
//         {
//             HotelId = 1,
//             RoomType = RoomType.Twin,
//             Price = 632
//         };
//
//         var hotel = new Hotel { Id = 1 };
//         var room = new Room
//         {
//             HotelId = 1,
//             RoomType = RoomType.Twin,
//             Price = 632
//         };
//
//         _hotelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);
//         _mapperMock.Setup(m => m.Map<Room>(creationDto)).Returns(room);
//         _roomRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Room>())).ReturnsAsync((Room?)null);
//
//         // Act
//         var result = await _roomService.CreateAsync(creationDto);
//
//         // Assert
//         result.Should().BeNull();
//     }
//
//
//     [Theory]
//     [InlineData(1)]
//     [InlineData(2)]
//     public async Task GetById_ShouldReturnDto_WhenRoomExists(int roomId)
//     {
//         //Arrange
//         var room = new Room()
//         {
//             Id = roomId,
//             HotelId = 1
//         };
//         _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId)).ReturnsAsync(room);
//         _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(new RoomDto()
//         {
//             Id = room.Id,
//             HotelId = 1,
//         });
//
//         //Act
//         var result = await _roomService.GetByIdAsync(roomId);
//
//         //Assert
//         result.Should().NotBeNull();
//         result.Id.Should().Be(roomId);
//         result.HotelId.Should().Be(1);
//     }
//
//     [Theory]
//     [InlineData(-1)]
//     [InlineData(null)]
//     public async Task GetById_ShouldReturnNull_WhenRoomDoesNotExist(int roomId)
//     {
//         //Arrange
//         _roomRepositoryMock.Setup(r => r.GetByIdAsync(roomId)).ReturnsAsync((Room?)null);
//
//         //Act
//         var result = await _roomService.GetByIdAsync(roomId);
//
//         //Assert
//         result.Should().BeNull();
//     }
//
//     [Fact]
//     public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
//     {
//         //Arrange
//         const int expectedRoomNumber = 101;
//
//         var updateDto = new RoomUpdateDto()
//         {
//             RoomId = 1,
//             RoomNumber = 100
//         };
//         var room = new Room()
//         {
//             Id = 1,
//         };
//         _mapperMock.Setup(m => m.Map<Room>(updateDto)).Returns(room);
//         _roomRepositoryMock.Setup(r => r.UpdateAsync(room)).ReturnsAsync(room);
//         _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(new RoomDto()
//         {
//             Id = 1,
//             RoomNumber = 101
//         });
//
//         //Act
//         var result = await _roomService.UpdateAsync(updateDto);
//
//         //Act
//         result.Should().NotBeNull();
//         result.RoomNumber.Should().Be(expectedRoomNumber);
//     }
//
//     [Fact]
//     public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
//     {
//         //Arrange
//         var updateDto = new RoomUpdateDto()
//         {
//             RoomId = 1,
//         };
//         var room = new Room()
//         {
//             Id = 1,
//         };
//         _mapperMock.Setup(m => m.Map<Room>(updateDto)).Returns(room);
//         _roomRepositoryMock.Setup(r => r.UpdateAsync(room)).ReturnsAsync((Room?)null);
//
//         //Act
//         var result = await _roomService.UpdateAsync(updateDto);
//
//         //Assert
//         result.Should().BeNull();
//     }
//
//     [Fact]
//     public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
//     {
//         //Arrange
//         const int roomId = 1;
//         var room = new Room()
//         {
//             Id = roomId,
//         };
//
//         _roomRepositoryMock.Setup(r => r.DeleteAsync(roomId)).ReturnsAsync(room);
//         _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(new RoomDto()
//         {
//             Id = roomId,
//         });
//
//         //Act
//         var result = await _roomService.DeleteAsync(roomId);
//
//         //Assert
//         result.Should().NotBeNull();
//         result.Id.Should().Be(roomId);
//     }
//
//     [Fact]
//     public async Task Delete_ShouldReturnNull_WhenDeletionFails()
//     {
//         //Arrange
//         const int roomId = -1;
//         _roomRepositoryMock.Setup(r => r.DeleteAsync(roomId)).ReturnsAsync((Room?)null);
//
//         //Act
//         var result = await _roomService.DeleteAsync(roomId);
//
//         //Assert
//         result.Should().BeNull();
//     }
// }

