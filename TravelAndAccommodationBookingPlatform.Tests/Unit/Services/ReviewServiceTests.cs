using AutoMapper;
using Azure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class ReviewServiceTests
{
    private readonly IReviewService _reviewService;
    private readonly Mock<IReviewRepository> _reviewRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IHotelRepository> _hotelRepositoryMock;
    private readonly Mock<IImageUploader> _imageUploaderMock;
    private readonly Mock<IMapper> _mapperMock;

    public ReviewServiceTests()
    {
        _reviewRepositoryMock = new Mock<IReviewRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _hotelRepositoryMock = new Mock<IHotelRepository>();
        _mapperMock = new Mock<IMapper>();
        _imageUploaderMock = new Mock<IImageUploader>();
        _reviewService = new ReviewService(_reviewRepositoryMock.Object, _userRepositoryMock.Object, _hotelRepositoryMock.Object, _imageUploaderMock.Object, _mapperMock.Object);
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
            .Setup(r => r.GetAllAsync(queryParams))
            .ReturnsAsync((reviews, pagination));
        _mapperMock.Setup(m => m.Map<IEnumerable<ReviewDto>>(reviews)).Returns(new List<ReviewDto>()
        {
            new ReviewDto() { UserId = 1, HotelId = 1, Comment = "Test", Rate = 5 },
            new ReviewDto() { UserId = 2, HotelId = 2, Comment = "Good", Rate = 4 }
        });

        // Act
        var (result, meta) = await _reviewService.GetAllAsync(queryParams);
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
            Rate = 4.5f
        };

        var files = new List<IFormFile>();
        var imagePath = "uploaded/image.jpg";

        var user = new User { Id = 1 };
        var hotel = new Hotel { Id = 2 };
        var review = new Review
        {
            ReviewId = 10,
            UserId = 1,
            HotelId = 2,
            Comment = creationDto.Comment,
            Rate = creationDto.Rate,
            FolderPath = imagePath,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(hotel);
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Reviews)).ReturnsAsync("some/image.jpg");
        _mapperMock.Setup(m => m.Map<Review>(creationDto)).Returns(review);
        _reviewRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Review>())).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto
        {
            ReviewId = 10,
            UserId = 1,
            HotelId = 2,
            Comment = creationDto.Comment,
            Rate = creationDto.Rate,
            ImagePath = imagePath
        });

        // Act
        var result = await _reviewService.CreateAsync(creationDto, files);

        // Assert
        result.Should().NotBeNull();
        result!.ReviewId.Should().Be(10);
        result.ImagePath.Should().Be(imagePath);
    }


    [Fact]
    public async Task Create_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        var files = new List<IFormFile>();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Hotel());

        // Act
        var result = await _reviewService.CreateAsync(creationDto, files);

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task Create_ShouldReturnNull_WhenHotelNotFound()
    {
        // Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        var files = new List<IFormFile>();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User());
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Hotel?)null);

        // Act
        var result = await _reviewService.CreateAsync(creationDto, files);

        // Assert
        result.Should().BeNull();
    }


    [Fact]
    public async Task Create_ShouldReturnNull_WhenReviewCreationFails()
    {
        // Arrange
        var creationDto = new ReviewCreationDto { UserId = 1, HotelId = 2 };
        var files = new List<IFormFile>();

        _userRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new User());
        _hotelRepositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(new Hotel());
        _mapperMock.Setup(m => m.Map<Review>(creationDto)).Returns(new Review());
        _imageUploaderMock.Setup(u => u.UploadImagesAsync(It.IsAny<List<IFormFile>>(), ImageEntityType.Reviews)).ReturnsAsync("some/image.jpg");
        _reviewRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Review>())).ReturnsAsync((Review?)null);

        // Act
        var result = await _reviewService.CreateAsync(creationDto, files);

        // Assert
        result.Should().BeNull();
    }


    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public async Task GetById_ShouldReturnDto_WhenReviewExists(int reviewId)
    {
        //Arrange
        var review = new Review() { ReviewId = reviewId };
        _reviewRepositoryMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto()
        {
            ReviewId = reviewId
        });

        //Act
        var result = await _reviewService.GetByIdAsync(reviewId);

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
        _reviewRepositoryMock.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync((Review?)null);

        //Act
        var result = await _reviewService.GetByIdAsync(reviewId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedDto_WhenSuccessful()
    {
        // Arrange
        const int expectedId = 1;

        var existingReview = new Review
        {
            ReviewId = expectedId,
            Comment = "Old Comment",
            Rate = 3.0f
        };

        var patchDoc = new JsonPatchDocument<ReviewUpdateDto>();
        patchDoc.Replace(r => r.Comment, "Updated Comment");

        var updatedDto = new ReviewUpdateDto
        {
            Comment = "Updated Comment",
            Rate = 3.0f
        };

        var updatedReview = new Review
        {
            ReviewId = expectedId,
            Comment = "Updated Comment",
            Rate = 3.0f,
            UpdatedAt = DateTime.UtcNow
        };

        var reviewDto = new ReviewDto
        {
            ReviewId = expectedId,
            Comment = "Updated Comment",
            Rate = 3.0f
        };

        _reviewRepositoryMock.Setup(r => r.GetByIdAsync(expectedId)).ReturnsAsync(existingReview);
        _mapperMock.Setup(m => m.Map<ReviewUpdateDto>(existingReview)).Returns(updatedDto);
        _mapperMock.Setup(m => m.Map(updatedDto, existingReview));
        _mapperMock.Setup(m => m.Map<ReviewDto>(existingReview)).Returns(reviewDto);

        // Act
        var result = await _reviewService.UpdateAsync(expectedId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result!.ReviewId.Should().Be(expectedId);
        result.Comment.Should().Be("Updated Comment");
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUpdateFails()
    {
        //Arrange
        const int expectedId = 1;
        var updateDto = new JsonPatchDocument<ReviewUpdateDto>() { };
        var review = new Review()
        {
            ReviewId = expectedId
        };
        _mapperMock.Setup(m => m.Map<Review>(updateDto)).Returns(review);

        //Act
        var result = await _reviewService.UpdateAsync(expectedId, updateDto);

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

        _reviewRepositoryMock.Setup(r => r.DeleteAsync(reviewId)).ReturnsAsync(review);
        _mapperMock.Setup(m => m.Map<ReviewDto>(review)).Returns(new ReviewDto
        {
            ReviewId = reviewId,
            UserId = 10,
            HotelId = 20,
            Comment = "Test",
            Rate = 4.5f
        });

        // Act
        var result = await _reviewService.DeleteAsync(reviewId);

        // Assert
        result.Should().NotBeNull();
        result.ReviewId.Should().Be(reviewId);
        result.Comment.Should().Be("Test");
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenDeletionFails()
    {
        // Arrange
        const int reviewId = -1;
        _reviewRepositoryMock.Setup(r => r.DeleteAsync(reviewId)).ReturnsAsync((Review?)null);

        // Act
        var result = await _reviewService.DeleteAsync(reviewId);

        // Assert
        result.Should().BeNull();
    }

}