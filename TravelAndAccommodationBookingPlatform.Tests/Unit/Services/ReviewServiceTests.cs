using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class ReviewServiceTests
{
    private readonly IReviewService _reviewService;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;

    public ReviewServiceTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _mapperMock = new Mock<IMapper>();
        _reviewService = new ReviewService(_reviewRepositoryMock.Object, _userRepositoryMock.Object, _hotelRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedReviewsAndPagination()
    {
        // Arrange
        var queryParams = new ReviewQueryParameters();
        var reviews = new List<Review>
        {
            new Review() { ReviewId = 1, UserId = 1, HotelId = 1, Comment = "Test", Rate = 5 },
            new Review() { ReviewId = 2, UserId = 2, HotelId = 2, Comment = "Good", Rate = 4 }
        };

        var pagination = new PaginationMetaData(2, 1, 10);

        _reviewRepositoryMock
            .Setup(r => r.GetAll(queryParams))
            .ReturnsAsync((reviews, pagination));
        _mapperMock.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews)).Returns(new List<ReviewDto>()
        {
            new ReviewDto() { UserId = 1, HotelId = 1, Comment = "Test", Rate = 5 },
            new ReviewDto() { UserId = 2, HotelId = 2, Comment = "Good", Rate = 4 }
        });

        // Act
        var (result, meta) = await _reviewService.GetAll(queryParams);
        result = result.ToList();

        // Assert
        result.Count().Should().Be(2);
        meta.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task Create_ShouldReturnReviewDto_WhenSuccessful()
    {
        // Arrange
        var creationDto = new ReviewCreationDto
        {
            UserId = 1,
            HotelId = 2,
            Comment = "Nice stay",
            Rate = 4.5f,
            ImagePath = "image.jpg"
        };

        var user = new User { Id = 1 };
        var hotel = new Hotel { Id = 2 };

        _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(user);
        _hotelRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync(hotel);

        var review = new Review
        {
            ReviewId = 10,
            UserId = 1,
            HotelId = 2,
            Comment = creationDto.Comment,
            Rate = creationDto.Rate,
            ImagePath = creationDto.ImagePath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _mapperMock.Setup(m => m.Map<Review>(creationDto)).Returns(review);

        _reviewRepositoryMock.Setup(r => r.Create(It.IsAny<Review>())).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto()
        {
            ReviewId = 10,
            UserId = 1,
            HotelId = 2,
            Comment = creationDto.Comment,
            Rate = creationDto.Rate,
            ImagePath = creationDto.ImagePath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        // Act
        var result = await _reviewService.Create(creationDto);

        // Assert
        result.Should().NotBeNull();
        result.ReviewId.Should().Be(10);
        result.UserId.Should().Be(1);
        result.HotelId.Should().Be(2);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenUserNotFound()
    {
        //Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync((User?)null);
        _hotelRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync(new Hotel());

        //Act
        var result = await _reviewService.Create(creationDto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenHotelNotFound()
    {
        //Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User());
        _hotelRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync((Hotel?)null);

        //Act
        var result = await _reviewService.Create(creationDto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenReviewCreationFails()
    {
        //Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        _userRepositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(new User());
        _hotelRepositoryMock.Setup(r => r.GetById(2)).ReturnsAsync(new Hotel());
        _reviewRepositoryMock.Setup(r => r.Create(It.IsAny<Review>())).ReturnsAsync((Review?)null);
        _mapperMock.Setup(m => m.Map<Review>(creationDto)).Returns(new Review());

        //Act
        var result = await _reviewService.Create(creationDto);


        //Assert
        result.Should().BeNull();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetById_ShouldReturnDto_WhenReviewExists(int reviewId)
    {
        //Arrange
        var review = new Review() { ReviewId = reviewId };
        _reviewRepositoryMock.Setup(r => r.GetById(reviewId)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto()
        {
            ReviewId = reviewId
        });

        //Act
        var result = await _reviewService.GetById(reviewId);

        //Assert
        result.Should().NotBeNull();
        result.ReviewId.Should().Be(reviewId);

    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_ShouldReturnNull_WhenReviewDoesNotExist(int reviewId)
    {
        //Arrange
        _reviewRepositoryMock.Setup(r => r.GetById(reviewId)).ReturnsAsync((Review?)null);

        //Act
        var result = await _reviewService.GetById(reviewId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        //Arrange
        const int expectedId = 1;
        var updateDto = new ReviewUpdateDto()
        {
            ReviewId = expectedId
        };
        var review = new Review()
        {
            ReviewId = expectedId
        };
        _mapperMock.Setup(m => m.Map<Review>(updateDto)).Returns(review);
        _reviewRepositoryMock.Setup(r => r.UpdateAsync(review)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto()
        {
            ReviewId = expectedId
        });

        //Act
        var result = await _reviewService.UpdateAsync(updateDto);

        //Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        //Arrange
        var updateDto = new ReviewUpdateDto()
        {
            ReviewId = 1
        };
        var review = new Review()
        {
            ReviewId = 1
        };
        _mapperMock.Setup(m => m.Map<Review>(updateDto)).Returns(review);
        _reviewRepositoryMock.Setup(r => r.UpdateAsync(review)).ReturnsAsync((Review?)null);

        //Act
        var result = await _reviewService.UpdateAsync(updateDto);

        //Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task Delete_ShouldReturnDto_WhenDeletionSucceeds()
    {
        // Arrange
        const int reviewId = 1;
        var review = new Review
        {
            ReviewId = reviewId,
            UserId = 10,
            HotelId = 20,
            Comment = "Test",
            Rate = 4.5f,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _reviewRepositoryMock.Setup(r => r.Delete(reviewId)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto
        {
            ReviewId = reviewId,
            UserId = 10,
            HotelId = 20,
            Comment = "Test",
            Rate = 4.5f
        });

        // Act
        var result = await _reviewService.Delete(reviewId);

        // Assert
        result.Should().NotBeNull();
        result!.ReviewId.Should().Be(reviewId);
        result.Comment.Should().Be("Test");
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int reviewId = -1;
        _reviewRepositoryMock.Setup(r => r.Delete(reviewId)).ReturnsAsync((Review?)null);

        // Act
        var result = await _reviewService.Delete(reviewId);

        // Assert
        result.Should().BeNull();
    }

}