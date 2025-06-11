using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common.DatabaseFactories;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class ReviewRepositoryTests : IDisposable
{
    private readonly IAppDbContext _context;
    private readonly IReviewRepository _reviewRepository;
    public ReviewRepositoryTests()
    {
        var dbFactory = new DbContextFactory();
        _context = dbFactory.Create(DatabaseType.InMemory);
        _reviewRepository = new ReviewRepository(_context);
    }

    private List<Review> _reviews = new List<Review>()
    {
        new Review
        {
            ReviewId = 1,
            UserId = 1,
            HotelId = 1,
            Comment = "Amazing service and beautiful view!",
            Rate = 4.8f,
            ImagePath = "images/reviews/review1.jpg",
            CreatedAt = new DateTime(2024, 12, 15),
            UpdatedAt = new DateTime(2024, 12, 15)
        },
        new Review
        {
            ReviewId = 2,
            UserId = 2,
            HotelId = 1,
            Comment = "Good location but noisy at night.",
            Rate = 3.5f,
            ImagePath = "images/reviews/review2.jpg",
            CreatedAt = new DateTime(2025, 1, 10),
            UpdatedAt = new DateTime(2025, 1, 10)
        },
        new Review
        {
            ReviewId = 3,
            UserId = 1,
            HotelId = 2,
            Comment = "Clean rooms and friendly staff.",
            Rate = 4.2f,
            ImagePath = null,
            CreatedAt = new DateTime(2025, 2, 20),
            UpdatedAt = new DateTime(2025, 2, 21)
        },
        new Review
        {
            ReviewId = 4,
            UserId = 3,
            HotelId = 2,
            Comment = "Mediocre experience overall.",
            Rate = 2.9f,
            ImagePath = "images/reviews/review4.jpg",
            CreatedAt = new DateTime(2025, 3, 5),
            UpdatedAt = new DateTime(2025, 3, 5)
        },
        new Review
        {
            ReviewId = 5,
            UserId = 2,
            HotelId = 3,
            Comment = "Best stay I’ve had in years!",
            Rate = 5.0f,
            ImagePath = "images/reviews/review5.jpg",
            CreatedAt = new DateTime(2025, 4, 18),
            UpdatedAt = new DateTime(2025, 4, 18)
        }
    };

    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnPagedReviews(int pageNumber, int pageSize)
    {
        //Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();

        var queryParameters = new ReviewQueryParameters()
        {
            Page = pageNumber,
            PageSize = pageSize
        };

        //Act 
        var (entities, paginationMetaData) = await _reviewRepository.GetAll(queryParameters);
        var result = entities.ToList();

        int expectedCount = Math.Max(0, Math.Min(result.Count, pageSize));

        //Assert
        result.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }
    [Theory]
    [InlineData(1, 10, null, null, null)]
    [InlineData(2, 5, 1, null, null)]
    [InlineData(1, 3, null, 2, null)]
    [InlineData(1, 10, null, null, 4.5f)]
    [InlineData(1, 2, 3, 1, 3.5f)]
    public async Task GetAll_WithQueryParameters_ShouldReturnFilteredReviews(int page, int pageSize, int? hotelId, int? userId, float? rating)
    {
        // Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();

        var queryParams = new ReviewQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            HotelId = hotelId,
            UserId = userId,
            Rating = rating
        };

        // Act
        var (entities, paginationMetaData) = await _reviewRepository.GetAll(queryParams);
        var resultList = entities.ToList();

        var expected = _reviews.AsQueryable();

        if (hotelId.HasValue)
            expected = expected.Where(r => r.HotelId == hotelId.Value);

        if (userId.HasValue)
            expected = expected.Where(r => r.UserId == userId.Value);

        if (rating.HasValue)
            expected = expected.Where(r => r.Rate == rating.Value);

        var expectedPaged = expected
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Assert
        resultList.Count.Should().Be(expectedPaged.Count);
        resultList.Should().BeEquivalentTo(expectedPaged);
        paginationMetaData.CurrentPage.Should().Be(page);
    }

    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefaults()
    {
        //Arrange

        //Act
        var (entities, paginationMetaData) = await _reviewRepository.GetAll(new ReviewQueryParameters());

        //Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);

    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnReview()
    {
        //Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();

        //Act
        var result = await _reviewRepository.GetById(_reviews[0].ReviewId);

        //Assert
        result.Should().BeEquivalentTo(_reviews[0]);

    }

    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound(int reviewId)
    {
        //Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();

        //Act
        var result = await _reviewRepository.GetById(reviewId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateReview_WithValidData_ShouldCreateReview()
    {
        var review = new Review
        {
            ReviewId = 4,
            UserId = 3,
            HotelId = 2,
            Comment = "Mediocre experience overall.",
            Rate = 2.9f,
            ImagePath = "images/reviews/review4.jpg",
            CreatedAt = new DateTime(2025, 3, 5),
            UpdatedAt = new DateTime(2025, 3, 5)
        };

        //Act
        var createResult = await _reviewRepository.Create(review);
        var saveChangesResult = await _reviewRepository.SaveChangesAsync();

        //Assert
        createResult.Should().BeEquivalentTo(createResult);
        saveChangesResult.Should().Be(1);
    }

    [Fact]
    public async Task UpdateReview_WithValidData_ShouldUpdateReview()
    {
        //Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();
        var review = await _reviewRepository.GetById(_reviews[0].ReviewId);
        if (review == null)
        {
            Assert.Fail();
        }
        //Act
        review.Comment = "Mediocre experience overall.";
        var updateResult = await _reviewRepository.UpdateAsync(review);
        var saveChangesResult = await _reviewRepository.SaveChangesAsync();

        //Assert
        updateResult.Should().BeEquivalentTo(updateResult);
        saveChangesResult.Should().Be(1);
    }

    [Fact]
    public async Task UpdateReview_WithInvalidData_ShouldReturnNotFound()
    {
        //Arrange
        var review = new Review
        {
            ReviewId = -1,
            UserId = 3,
            HotelId = 2,
            Comment = "Mediocre experience overall.",
            Rate = 2.9f,
            ImagePath = "images/reviews/review4.jpg",
            CreatedAt = new DateTime(2025, 3, 5),
            UpdatedAt = new DateTime(2025, 3, 5)
        };

        //Act
        var updateResult = await _reviewRepository.UpdateAsync(review);
        var saveChangesResult = await _reviewRepository.SaveChangesAsync();

        //Assert
        updateResult.Should().BeNull();
        saveChangesResult.Should().Be(0);

    }

    [Fact]
    public async Task DeleteReview_WithValidData_ShouldDeleteReview()
    {
        //Arrange
        await _context.Reviews.AddRangeAsync(_reviews);
        await _context.SaveChangesAsync();
        var review = await _reviewRepository.GetById(_reviews[0].ReviewId);
        if (review == null)
        {
            Assert.Fail();
        }

        //Act
        var deleteResult = await _reviewRepository.Delete(review.ReviewId);
        var saveChangesResult = await _reviewRepository.SaveChangesAsync();
        var getResult = await _reviewRepository.GetById(review.ReviewId);

        //Assert
        deleteResult.Should().BeEquivalentTo(review);
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();
    }

    [Fact]
    public async Task DeleteReview_WithInvalidId_ShouldReturnNotFound()
    {
        //Arrange

        //Act
        var deleteResult = await _reviewRepository.Delete(-1);
        var saveChangesResult = await _reviewRepository.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        //Arrange
        var reviews = new List<Review>
        {
            new Review
            {
                ReviewId = 2,
                UserId = 2,
                HotelId = 1,
                Comment = "Good location but noisy at night.",
                Rate = 3.5f,
                ImagePath = "images/reviews/review2.jpg",
                CreatedAt = new DateTime(2025, 1, 10),
                UpdatedAt = new DateTime(2025, 1, 10)
            },
            new Review
            {
                ReviewId = 3,
                UserId = 1,
                HotelId = 2,
                Comment = "Clean rooms and friendly staff.",
                Rate = 4.2f,
                ImagePath = null,
                CreatedAt = new DateTime(2025, 2, 20),
                UpdatedAt = new DateTime(2025, 2, 21)
            }
        };

        //Act
        await _context.Reviews.AddRangeAsync(reviews);
        var saveChangesResult = await _context.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(reviews.Count);
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