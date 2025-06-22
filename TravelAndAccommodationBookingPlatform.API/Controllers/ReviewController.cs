using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using TravelAndAccommodationBookingPlatform.Application.DTOs.Review;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller responsible for managing hotel reviews.
/// </summary>
[ApiController]
[Route("[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly ILogger<ReviewController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewController"/> class.
    /// </summary>
    /// <param name="reviewService">The review service.</param>
    /// <param name="logger">The logger instance.</param>
    public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
    {
        _reviewService = reviewService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a list of reviews with optional filtering and pagination.
    /// </summary>
    /// <param name="reviewQueryParameters">Query parameters for filtering and pagination.</param>
    /// <returns>A list of reviews and pagination metadata in headers.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<ReviewDto>>> GetReviews([FromQuery] ReviewQueryParameters reviewQueryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (reviews, metaData) = await _reviewService.GetAllAsync(reviewQueryParameters);
            Response.Headers.Append("Reviews-pagination", JsonSerializer.Serialize(metaData));
            return Ok(reviews);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Fail to get reviews.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Creates a new review with one or more image files.
    /// </summary>
    /// <param name="reviewDto">The review creation data.</param>
    /// <param name="files">List of image files to attach to the review.</param>
    /// <returns>The created review.</returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewDto>> PostReview([FromForm] ReviewCreationDto reviewDto, List<IFormFile> files)
    {
        try
        {
            if (files.Count == 0)
            {
                return BadRequest("At least one hotel thumbnail is required.");
            }

            var createdReview = await _reviewService.CreateAsync(reviewDto, files);
            if (createdReview == null)
            {
                return StatusCode(500, "Failed to create review.");
            }

            return CreatedAtAction(nameof(GetReviewById), new { id = createdReview.Id }, createdReview);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Fail to create review.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Retrieves a review by its unique ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to retrieve.</param>
    /// <returns>The review with the specified ID.</returns>
    [HttpGet("{reviewId}")]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewDto>> GetReviewById(int reviewId)
    {
        try
        {
            var review = await _reviewService.GetByIdAsync(reviewId);
            if (review is null)
            {
                return NotFound("Review not found.");
            }

            return Ok(review);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Fail to get review.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Partially updates a review using JSON Patch.
    /// </summary>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="reviewDto">The patch document containing changes.</param>
    /// <returns>The updated review.</returns>
    [Authorize]
    [HttpPatch]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewDto>> PatchReview(int reviewId, JsonPatchDocument<ReviewUpdateDto> reviewDto)
    {
        try
        {
            var updatedReview = await _reviewService.UpdateAsync(reviewId, reviewDto);
            if (updatedReview is null)
            {
                return NotFound("Review not found.");
            }

            return Ok(updatedReview);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Fail to update review.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Deletes a review by its ID.
    /// </summary>
    /// <param name="reviewId">The ID of the review to delete.</param>
    /// <returns>The deleted review.</returns>
    [Authorize]
    [HttpDelete]
    [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReviewDto>> DeleteReview(int reviewId)
    {
        try
        {
            var deletedReview = await _reviewService.DeleteAsync(reviewId);
            if (deletedReview is null)
            {
                return NotFound("Review not found.");
            }

            return Ok(deletedReview);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Fail to delete review.");
            return StatusCode(500);
        }
    }

    /// <summary>
    /// Get Review images path by reviewId
    /// </summary>
    /// <param name="reviewId">The ID of the review you want</param>
    /// <returns></returns>
    [HttpGet("api/review/{reviewId}/images")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> GetHotelImages(int reviewId)
    {
        try
        {
            var images = await _reviewService.GetImagesPathAsync(reviewId);
            return Ok(images);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, $"Failed to get images for review with ID {reviewId}.");
            return StatusCode(500, "An unexpected error occurred while retrieving review images.");
        }
    }
}