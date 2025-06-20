using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TravelAndAccommodationBookingPlatform.API.Common.Interfaces;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;

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
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user service for managing users.</param>
    /// <param name="jwtProvider">Provides JWT configuration for token generation.</param>
    public UserController(
        IUserService userService,
        IJwtProvider jwtProvider)
    {
        _userService = userService;
        _secretKey = jwtProvider.SecretKey;
        _issuer = jwtProvider.Issuer;
        _audience = jwtProvider.Audience;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var (users, metaData) = await _userService.GetAllAsync(userQueryParameters);
        Response.Headers.Append("User-Pagination", JsonSerializer.Serialize(metaData));
        return Ok(users);
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
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
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
        var createResult = await _userService.CreateAsync(user);
        if (createResult == null)
        {
            return BadRequest();
        }
        return Ok(createResult);
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
        var updateResult = await _userService.UpdateAsync(id, user);
        if (updateResult == null)
        {
            return BadRequest();
        }
        return Ok(updateResult);
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token if credentials are valid.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="password">The user's password.</param>
    /// <returns>A JWT token if authentication succeeds; otherwise, a 401 Unauthorized response.</returns>
    [HttpPost("Login")]
    public async Task<ActionResult<string>> Login(string email, string password)
    {
        var user = await _userService.ValidateCredentialsAsync(email, password);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }

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
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );
        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(tokenString);
    }
}