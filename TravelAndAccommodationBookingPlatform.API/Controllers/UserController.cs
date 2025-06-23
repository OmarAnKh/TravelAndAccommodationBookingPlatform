using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TravelAndAccommodationBookingPlatform.API.Common.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.DTOs.RefreshToken;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.API.Controllers;

/// <summary>
/// Controller for managing users.
/// Provides endpoints for CRUD operations and user authentication.
/// </summary>
[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    private readonly IUserService _userService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly ILogger<UserController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service for managing users.</param>
    /// <param name="jwtProvider">Provides JWT configuration for token generation.</param>
    /// <param name="logger">Logger for User controller.</param>
    /// <param name="refreshTokenService">Refresh token for user controller</param>
    public UserController(
        IUserService userService,
        IJwtProvider jwtProvider
        , ILogger<UserController> logger, IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _secretKey = jwtProvider.SecretKey;
        _issuer = jwtProvider.Issuer;
        _audience = jwtProvider.Audience;
        _logger = logger;
        _refreshTokenService = refreshTokenService;

    }

    /// <summary>
    /// Retrieves a paginated list of users based on query parameters.
    /// </summary>
    /// <param name="userQueryParameters">The query parameters for filtering and pagination.</param>
    /// <returns>A list of user DTOs with pagination metadata in the response headers.</returns>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] UserQueryParameters userQueryParameters)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (users, metaData) = await _userService.GetAllAsync(userQueryParameters);
            Response.Headers.Append("User-Pagination", JsonSerializer.Serialize(metaData));
            return Ok(users);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while getting users.");
            return StatusCode(500, "An unexpected error occurred.");
        }

    }

    /// <summary>
    /// Retrieves a specific user by ID.
    /// </summary>
    /// <param name="id">The ID of the user to retrieve.</param>
    /// <returns>The user DTO if found; otherwise, a 404 Not Found response.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while getting user.");
            return StatusCode(500, "An Unexpected error occurred.");
        }

    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user creation DTO containing new user details.</param>
    /// <returns>The created user DTO if successful; otherwise, a 400 Bad Request response.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDto>> PostUser(UserCreationDto user)
    {
        try
        {
            var createResult = await _userService.CreateAsync(user);
            if (createResult == null)
            {
                return BadRequest();
            }
            return Ok(createResult);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while posting user.");
            return StatusCode(500, "An Unexpected error occurred.");
        }

    }

    /// <summary>
    /// Updates an existing user using a JSON Patch document.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="user">The JSON Patch document with the fields to update.</param>
    /// <returns>The updated user DTO if successful; otherwise, a 400 Bad Request response.</returns>
    [Authorize]
    [HttpPatch]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserDto>> PatchUser(int id, JsonPatchDocument<UserUpdateDto> user)
    {
        try
        {
            var updateResult = await _userService.UpdateAsync(id, user);
            if (updateResult == null)
            {
                return BadRequest();
            }
            return Ok(updateResult);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while updating user.");
            return StatusCode(500, "An Unexpected error occurred.");
        }

    }

    /// <summary>
    /// Authenticates a user and returns JWT access & refresh tokens if credentials are valid.
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<object>> Login(string email, string password)
    {
        try
        {
            var user = await _userService.ValidateCredentialsAsync(email, password);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = user.Id,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenService.CreateTokenAsync(refreshToken);

            return Ok(new
            {
                accessToken,
                refreshToken = refreshToken.Token
            });
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while logging in.");
            return StatusCode(500, "An Unexpected error occurred.");
        }
    }

    /// <summary>
    /// Refreshes the access token using a valid refresh token.
    /// </summary>
    /// <param name="request">Contains expired access token and refresh token.</param>
    /// <returns>New access and refresh tokens if successful.</returns>
    [HttpPost("RefreshToken")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<object>> RefreshToken([FromBody] TokenRefreshRequest request)
    {
        try
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userId = int.Parse(principal.FindFirst("UserId")?.Value ?? "0");

            var existingToken = await _refreshTokenService.GetValidTokenAsync(request.RefreshToken);
            if (existingToken == null || existingToken.UserId != userId)
            {
                return Unauthorized("Invalid refresh token");
            }

            var user = await _userService.GetByIdAsync(userId);
            var newAccessToken = GenerateAccessToken(user!);

            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            await _refreshTokenService.RevokeTokenAsync(existingToken.Token);
            await _refreshTokenService.CreateTokenAsync(newRefreshToken);

            return Ok(new
            {
                accessToken = newAccessToken,
                refreshToken = newRefreshToken.Token
            });
        }
        catch (SecurityTokenException)
        {
            return Unauthorized("Invalid access token");
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unexpected error occurred while refreshing token.");
            return StatusCode(500, "An Unexpected error occurred.");
        }
    }

    private string GenerateAccessToken(UserDto user)
    {
        var key = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Email", user.Email),
            new Claim("Role", user.Role.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(_secretKey))
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        if (securityToken is not JwtSecurityToken jwtToken ||
            !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }

}