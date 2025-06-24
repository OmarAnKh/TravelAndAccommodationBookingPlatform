using FluentAssertions;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;
using TravelAndAccommodationBookingPlatform.Infrastructure.Data;
using TravelAndAccommodationBookingPlatform.Infrastructure.Repositories;
using TravelAndAccommodationBookingPlatform.Tests.common;
using TravelAndAccommodationBookingPlatform.Tests.enums;

namespace TravelAndAccommodationBookingPlatform.Tests.Integration.Repositories;

public class UserRepositoryTests : IDisposable
{
    private readonly IAppDbContext _context;
    private readonly IUserRepository _userRepository;

    private readonly List<User> _users = new()
    {
        new User { Username = "alice", Password = "pass123", Email = "alice@example.com" },
        new User { Username = "bob", Password = "pass123", Email = "bob@example.com" },
        new User { Username = "carol", Password = "pass123", Email = "carol@example.com" },
        new User { Username = "dave", Password = "pass123", Email = "dave@example.com" },
        new User { Username = "eve", Password = "pass123", Email = "eve@example.com" }
    };

    public UserRepositoryTests()
    {
        var dbContextFactory = new DbContextFactory();
        _context = dbContextFactory.Create(DatabaseType.InMemory);
        _userRepository = new UserRepository(_context);
    }
    [Theory]
    [InlineData(1, 10)]
    [InlineData(2, 2)]
    [InlineData(2, 10)]
    public async Task GetAll_ReturnPagedUsers(int pageNumber, int pageSize)
    {
        //Arrange
        await _context.Users.AddRangeAsync(_users);
        await _context.SaveChangesAsync();

        var queryParameters = new UserQueryParameters()
        {
            PageSize = pageSize,
            Page = pageNumber
        };

        //Act
        var (entities, paginationMetaData) = await _userRepository.GetAllAsync(queryParameters);
        var result = entities.ToList();

        var expectedCount = Math.Max(0, Math.Min(result.Count, pageSize));

        //Assert
        result.Count.Should().Be(expectedCount);
        paginationMetaData.CurrentPage.Should().Be(pageNumber);
    }

    [Theory]
    [InlineData(1, 2, "a", null, null, null, null, null)]
    [InlineData(1, 1, null, "bob", null, null, null, null)]
    [InlineData(1, 10, null, null, UserRole.Admin, null, null, null)]
    [InlineData(1, 10, null, null, null, "1990-01-01", null, null)]
    [InlineData(1, 10, null, null, null, null, null, "2025-06-01")]
    public async Task GetAll_WithQueryParameters_ShouldReturnFilteredUsers(int page, int pageSize, string username, string email, UserRole? role, string minBirthDate, string maxBirthDate, string createdBefore)
    {
        // Arrange
        await _context.Users.AddRangeAsync(_users);
        await _context.SaveChangesAsync();

        var queryParams = new UserQueryParameters
        {
            Page = page,
            PageSize = pageSize,
            Username = username,
            Email = email,
            Role = role,
            MinBirthDate = string.IsNullOrEmpty(minBirthDate) ? null : DateTime.Parse(minBirthDate),
            MaxBirthDate = string.IsNullOrEmpty(maxBirthDate) ? null : DateTime.Parse(maxBirthDate),
            CreatedBefore = string.IsNullOrEmpty(createdBefore) ? null : DateTime.Parse(createdBefore),
        };

        // Act
        var (result, pagination) = await _userRepository.GetAllAsync(queryParams);
        var expected = _users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(username))
            expected = expected.Where(u => u.Username.Contains(username));

        if (!string.IsNullOrWhiteSpace(email))
            expected = expected.Where(u => u.Email.Contains(email));

        if (role.HasValue)
            expected = expected.Where(u => u.Role == role);

        if (!string.IsNullOrEmpty(minBirthDate))
            expected = expected.Where(u => u.BirthDate >= DateTime.Parse(minBirthDate));

        if (!string.IsNullOrEmpty(maxBirthDate))
            expected = expected.Where(u => u.BirthDate <= DateTime.Parse(maxBirthDate));

        if (!string.IsNullOrEmpty(createdBefore))
            expected = expected.Where(u => u.CreatedAt <= DateTime.Parse(createdBefore));

        var expectedPaged = expected
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Assert
        result.Should().BeEquivalentTo(expectedPaged);
        pagination.CurrentPage.Should().Be(page);
        pagination.PageSize.Should().Be(pageSize);
    }


    [Fact]
    public async Task GetAll_WithEmptyObject_ShouldUseDefault()
    {
        //Arrange

        //Act
        var (entities, paginationMetaData) = await _userRepository.GetAllAsync(new UserQueryParameters());

        //Assert
        paginationMetaData.CurrentPage.Should().Be(1);
        paginationMetaData.PageSize.Should().Be(10);
    }


    [Fact]
    public async Task GetById_WithValidId_ShouldReturnUser()
    {
        //Arrange
        await _context.Users.AddRangeAsync(_users);
        await _context.SaveChangesAsync();

        //Act
        var result = await _userRepository.GetByIdAsync(_users[0].Id);

        //Assert
        result.Should().BeEquivalentTo(_users[0]);
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(null)]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound(int roomId)
    {
        //Arrange
        await _context.Users.AddRangeAsync(_users);
        await _context.SaveChangesAsync();

        //Act
        var result = await _userRepository.GetByIdAsync(roomId);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldReturnCreatedUser()
    {
        //Arrange
        var user = new User { Id = 3, Username = "carol", Password = "pass123", Email = "carol@example.com" };

        //Act
        var createResult = await _userRepository.CreateAsync(user);
        var saveChangesResult = await _userRepository.SaveChangesAsync();

        //Assert
        createResult.Should().BeEquivalentTo(user);
        saveChangesResult.Should().Be(1);

    }


    [Fact]
    public async Task DeleteUser_WithValidData_ShouldReturnDeletedUser()
    {
        //Arrange
        await _context.Users.AddRangeAsync(_users);
        await _context.SaveChangesAsync();
        var user = await _userRepository.GetByIdAsync(_users[0].Id);
        if (user == null)
        {
            Assert.Fail();
        }

        //Act
        var deleteResult = await _userRepository.DeleteAsync(user.Id);
        var saveChangesResult = await _userRepository.SaveChangesAsync();
        var getResult = await _userRepository.GetByIdAsync(_users[0].Id);

        //Assert
        deleteResult.Should().BeEquivalentTo(user);
        saveChangesResult.Should().Be(1);
        getResult.Should().BeNull();

    }

    [Fact]
    public async Task DeleteUser_WithInvalidId_ShouldReturnNotFound()
    {
        //Arrange

        //Act
        var deleteResult = await _userRepository.DeleteAsync(-1);
        var saveChangesResult = await _userRepository.SaveChangesAsync();

        //Assert
        deleteResult.Should().BeNull();
        saveChangesResult.Should().Be(0);
    }

    [Fact]
    public async Task SaveChangesAsync_WithMultipleValues_ShouldReturnTheNumberOfChanges()
    {
        //Arrange 
        var users = new List<User>
        {
            new User { Username = "bob", Password = "pass123", Email = "bob@example.com" },
            new User { Username = "carol", Password = "pass123", Email = "carol@example.com" },
            new User { Username = "dave", Password = "pass123", Email = "dave@example.com" },
            new User { Username = "eve", Password = "pass123", Email = "eve@example.com" }
        };

        //Act
        await _context.Users.AddRangeAsync(users);
        var saveChangesResult = await _userRepository.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(users.Count);

    }

    [Fact]
    public async Task SaveChangesAsync_WithNoChanges_ShouldReturnZeroChange()
    {
        //Arrange

        //Act
        var saveChangesResult = await _userRepository.SaveChangesAsync();

        //Assert
        saveChangesResult.Should().Be(0);
    }
    public void Dispose()
    {
        _context.Dispose();
    }
}