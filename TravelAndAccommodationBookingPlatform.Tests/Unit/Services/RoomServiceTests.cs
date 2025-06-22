using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Room;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class RoomServiceTests
{
    private readonly IRoomService _roomService;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IImageUploader> _imageUploaderMock;
    private readonly Mock<IMapper> _mapperMock;

    public RoomServiceTests()
    {
        _roomRepositoryMock = new Mock<IRoomRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _imageUploaderMock = new Mock<IImageUploader>();
        _mapperMock = new Mock<IMapper>();

        _roomService = new RoomService(
            _roomRepositoryMock.Object,
            _hotelRepositoryMock.Object,
            _imageUploaderMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedRoomsAndPagination()
    {
        var queryParams = new RoomQueryParameters();
        var rooms = new List<Room>
        {
            new Room { Id = 1, HotelId = 1, RoomType = RoomType.Deluxe, Price = 100 },
            new Room { Id = 2, HotelId = 1, RoomType = RoomType.Single, Price = 70 }
        };
        var pagination = new PaginationMetaData(2, 1, 10);

        _roomRepositoryMock.Setup(r => r.GetAllAsync(queryParams))
            .ReturnsAsync((rooms, pagination));

        _mapperMock.Setup(m => m.Map<IEnumerable<RoomDto>>(rooms)).Returns(new List<RoomDto>
        {
            new RoomDto { Id = 1, HotelId = 1, RoomType = RoomType.Deluxe, Price = 100 },
            new RoomDto { Id = 2, HotelId = 1, RoomType = RoomType.Single, Price = 70 }
        });

        var (result, meta) = await _roomService.GetAllAsync(queryParams);
        result.Count().Should().Be(2);
        meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Create_ShouldReturnRoomDto_WhenSuccessful()
    {
        var creationDto = new RoomCreationDto { HotelId = 1, CustomRoomTypeName = "Suite", Price = 150 };
        var hotel = new Hotel { Id = 1 };
        var files = new List<IFormFile>();
        var room = new Room { Id = 5, HotelId = 1, CustomRoomTypeName = "Suite", Price = 150 };
        var expectedDto = new RoomDto { Id = 5, HotelId = 1, CustomRoomTypeName = "Suite", Price = 150 };

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<Room>(creationDto)).Returns(room);
        _imageUploaderMock.Setup(i => i.UploadImagesAsync(files, ImageEntityType.Rooms)).ReturnsAsync("some-path");
        _roomRepositoryMock.Setup(r => r.CreateAsync(room)).ReturnsAsync(room);
        _roomRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(expectedDto);

        var result = await _roomService.CreateAsync(creationDto, files);
        result.Should().NotBeNull();
        result.Id.Should().Be(5);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenHotelDoesNotExist()
    {
        var creationDto = new RoomCreationDto { HotelId = 999 };
        var files = new List<IFormFile>();

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Hotel?)null);
        var result = await _roomService.CreateAsync(creationDto, files);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenRepositoryReturnsNull()
    {
        var creationDto = new RoomCreationDto { HotelId = 1 };
        var files = new List<IFormFile>();
        var hotel = new Hotel { Id = 1 };
        var room = new Room { HotelId = 1 };

        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(hotel);
        _mapperMock.Setup(m => m.Map<Room>(creationDto)).Returns(room);
        _imageUploaderMock.Setup(i => i.UploadImagesAsync(files, ImageEntityType.Rooms)).ReturnsAsync("some-path");
        _roomRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Room>())).ReturnsAsync((Room?)null);

        var result = await _roomService.CreateAsync(creationDto, files);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ShouldReturnDto_WhenRoomExists()
    {
        var room = new Room { Id = 1, HotelId = 1 };
        var dto = new RoomDto { Id = 1, HotelId = 1 };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(dto);

        var result = await _roomService.GetByIdAsync(1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Room?)null);
        var result = await _roomService.GetByIdAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDto_WhenSuccessful()
    {
        var room = new Room { Id = 1 };
        var dto = new RoomDto { Id = 1 };

        _roomRepositoryMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(dto);

        var result = await _roomService.DeleteAsync(1);
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        _roomRepositoryMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync((Room?)null);
        var result = await _roomService.DeleteAsync(999);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetImagesPathAsync_ShouldReturnList_WhenRoomExists()
    {
        var room = new Room { Id = 1, Thumbnail = "some-path" };
        var imageUrls = new List<string> { "url1", "url2" };

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _imageUploaderMock.Setup(i => i.GetImageUrlsAsync("some-path")).ReturnsAsync(imageUrls);

        var result = await _roomService.GetImagesPathAsync(1);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetImagesPathAsync_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Room?)null);
        var result = await _roomService.GetImagesPathAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAvailableRoomsAsync_ShouldReturnMappedDtos()
    {
        var rooms = new List<Room>
        {
            new Room { Id = 1 },
            new Room { Id = 2 }
        };

        _roomRepositoryMock.Setup(r => r.GetAvailableRoomsAsync(1)).ReturnsAsync(rooms);
        _mapperMock.Setup(m => m.Map<IEnumerable<RoomDto>>(rooms)).Returns(new List<RoomDto>
        {
            new RoomDto { Id = 1 },
            new RoomDto { Id = 2 }
        });

        var result = await _roomService.GetAvailableRoomsAsync(1);
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnPatchedRoom_WhenRoomExists()
    {
        var room = new Room { Id = 1, RoomNumber = 100 };
        var patchDoc = new JsonPatchDocument<RoomUpdateDto>();
        patchDoc.Replace(r => r.RoomNumber, 101);

        _roomRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map<RoomUpdateDto>(room)).Returns(new RoomUpdateDto { RoomNumber = 100 });
        _mapperMock.Setup(m => m.Map(It.IsAny<RoomUpdateDto>(), room)).Verifiable();
        _roomRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<RoomDto>(room)).Returns(new RoomDto { Id = 1, RoomNumber = 101 });

        var result = await _roomService.UpdateAsync(1, patchDoc);
        result.Should().NotBeNull();
        result.RoomNumber.Should().Be(101);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenRoomDoesNotExist()
    {
        var patchDoc = new JsonPatchDocument<RoomUpdateDto>();
        _roomRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Room?)null);
        var result = await _roomService.UpdateAsync(999, patchDoc);
        result.Should().BeNull();
    }
}