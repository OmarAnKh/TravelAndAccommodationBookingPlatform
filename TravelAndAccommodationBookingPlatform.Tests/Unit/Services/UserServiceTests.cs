using AutoMapper;
using FluentAssertions;
using Moq;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Application.Services;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Tests.Unit.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _sut = new UserService(_userRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnMappedUsersWithPaginationMetaData()
    {
        // Arrange
        var queryParams = new UserQueryParameters();
        var users = new List<User> { new() { Id = 1, Username = "john", Email = "john@example.com" } };
        var metadata = new PaginationMetaData(1, 1, 10);
        _userRepoMock.Setup(x => x.GetAll(queryParams)).ReturnsAsync((users, metadata));
        _mapperMock.Setup(x => x.Map<IEnumerable<UserDto>>(users)).Returns(users.Select(u => new UserDto { Id = u.Id }));

        // Act
        var (result, meta) = await _sut.GetAll(queryParams);

        // Assert
        result.Should().HaveCount(1);
        meta.Should().BeSameAs(metadata);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenUserExists()
    {
        
        //Arrange
        var dto = new UserCreationDto { Username = "existing", Email = "existing@email.com" };
        _userRepoMock.Setup(x => x.GetByUsername(dto.Username)).ReturnsAsync(new User());
        _userRepoMock.Setup(x => x.GetByEmail(dto.Email)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.Create(dto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Create_ShouldCreateUser_WhenValid()
    {
        //Arrange
        var dto = new UserCreationDto
        {
            Username = "newuser",
            Email = "new@email.com",
            Password = "123",
            BirthDate = DateTime.UtcNow
        };
        var user = new User { Id = 1 };
        var createdUser = new User { Id = 1 };

        _userRepoMock.Setup(x => x.GetByUsername(dto.Username)).ReturnsAsync((User)null);
        _userRepoMock.Setup(x => x.GetByEmail(dto.Email)).ReturnsAsync((User)null);
        _mapperMock.Setup(x => x.Map<User>(dto)).Returns(user);
        _userRepoMock.Setup(x => x.Create(user)).ReturnsAsync(createdUser);
        _mapperMock.Setup(x => x.Map<UserDto>(createdUser)).Returns(new UserDto { Id = 1 });

        //Act
        var result = await _sut.Create(dto);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    
    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUsernameExists()
    {
        //Arrange
        var dto = new UserUpdateDto { Id = 1, Username = "taken" };
        _userRepoMock.Setup(x => x.GetByUsername(dto.Username)).ReturnsAsync(new User { Id = 2 });

        //Act
        var result = await _sut.UpdateAsync(dto);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedUser_WhenValid()
    {
        //Arrange
        var dto = new UserUpdateDto { Id = 1, Username = "updated" };
        var user = new User { Id = 1 };
        var updatedUser = new User { Id = 1 };

        _userRepoMock.Setup(x => x.GetByUsername(dto.Username)).ReturnsAsync((User)null);
        _mapperMock.Setup(x => x.Map<User>(dto)).Returns(user);
        _userRepoMock.Setup(x => x.UpdateAsync(user)).ReturnsAsync(updatedUser);
        _mapperMock.Setup(x => x.Map<UserDto>(updatedUser)).Returns(new UserDto { Id = 1 });

        //Act
        var result = await _sut.UpdateAsync(dto);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    
    [Theory]
    [InlineData(1)]
    public async Task GetById_ShouldReturnMappedUser_WhenExists(int id)
    {
        //Arrange
        var user = new User { Id = id };
        var dto = new UserDto { Id = id };
        _userRepoMock.Setup(x => x.GetById(id)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(dto);

        //Act
        var result = await _sut.GetById(id);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetById(-1)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetById(-1);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenUserNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.Delete(-1)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.Delete(-1);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDeletedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 1 };
        _userRepoMock.Setup(x => x.Delete(1)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 1 });

        //Act
        var result = await _sut.Delete(1);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnMappedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        _userRepoMock.Setup(x => x.GetByEmail(user.Email)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 1, Email = "test@example.com" });

        //Act
        var result = await _sut.GetByEmail(user.Email);

        //Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnNull_WhenNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetByEmail("none")).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetByEmail("none");

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnMappedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 2, Username = "ahmad" };
        _userRepoMock.Setup(x => x.GetByUsername(user.Username)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 2, Username = "ahmad" });

        //Act
        var result = await _sut.GetByUsername(user.Username);
        
        //Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnNull_WhenNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetByUsername("notfound")).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetByUsername("notfound");

        
        //Assert
        result.Should().BeNull();
    }
}