using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
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
        _userRepoMock.Setup(x => x.GetAllAsync(queryParams)).ReturnsAsync((users, metadata));
        _mapperMock.Setup(x => x.Map<IEnumerable<UserDto>>(users)).Returns(users.Select(u => new UserDto { Id = u.Id }));

        // Act
        var (result, meta) = await _sut.GetAllAsync(queryParams);

        // Assert
        result.Should().HaveCount(1);
        meta.Should().BeSameAs(metadata);
    }

    [Fact]
    public async Task Create_ShouldReturnNull_WhenUserExists()
    {

        //Arrange
        var dto = new UserCreationDto { Username = "existing", Email = "existing@email.com" };
        _userRepoMock.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync(new User());
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.CreateAsync(dto);

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

        _userRepoMock.Setup(x => x.GetByUsernameAsync(dto.Username)).ReturnsAsync((User)null);
        _userRepoMock.Setup(x => x.GetByEmailAsync(dto.Email)).ReturnsAsync((User)null);
        _mapperMock.Setup(x => x.Map<User>(dto)).Returns(user);
        _userRepoMock.Setup(x => x.CreateAsync(user)).ReturnsAsync(createdUser);
        _mapperMock.Setup(x => x.Map<UserDto>(createdUser)).Returns(new UserDto { Id = 1 });

        //Act
        var result = await _sut.CreateAsync(dto);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        const int userId = 1;
        const string newUsername = "updated";

        var patchDoc = new JsonPatchDocument<UserUpdateDto>();
        patchDoc.Replace(x => x.Username, newUsername);

        _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.UpdateAsync(userId, patchDoc);

        // Assert
        result.Should().BeNull();
        _userRepoMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenUsernameExists()
    {
        // Arrange
        const int userId = 1;
        const string takenUsername = "taken";

        var existingUser = new User { Id = userId, Username = "original" };
        var userUpdateDto = new UserUpdateDto { Id = userId, Username = takenUsername };

        var patchDoc = new JsonPatchDocument<UserUpdateDto>();
        patchDoc.Replace(x => x.Username, takenUsername);

        _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mapperMock.Setup(x => x.Map<UserUpdateDto>(existingUser)).Returns(userUpdateDto);

        // Act
        var result = await _sut.UpdateAsync(userId, patchDoc);

        // Assert
        result.Should().BeNull();
        _userRepoMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepoMock.Verify(x => x.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnUpdatedUser_WhenValid()
    {
        // Arrange
        const int userId = 1;
        const string originalUsername = "original";
        const string updatedUsername = "updated";

        var existingUser = new User { Id = userId, Username = originalUsername };
        var userUpdateDto = new UserUpdateDto { Id = userId, Username = updatedUsername };
        var expectedUserDto = new UserDto { Id = userId, Username = updatedUsername };

        var patchDoc = new JsonPatchDocument<UserUpdateDto>();
        patchDoc.Replace(x => x.Username, updatedUsername);

        _userRepoMock.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(existingUser);
        _mapperMock.Setup(x => x.Map<UserUpdateDto>(existingUser)).Returns(userUpdateDto);
        _userRepoMock.Setup(x => x.GetByUsernameAsync(updatedUsername)).ReturnsAsync((User?)null);
        _mapperMock.Setup(x => x.Map(userUpdateDto, existingUser));
        _mapperMock.Setup(x => x.Map<UserDto>(existingUser)).Returns(expectedUserDto);
        _userRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _sut.UpdateAsync(userId, patchDoc);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Username.Should().Be(updatedUsername);
        _userRepoMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _userRepoMock.Verify(x => x.GetByUsernameAsync(updatedUsername), Times.Once);
        _userRepoMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }


    [Theory]
    [InlineData(1)]
    public async Task GetById_ShouldReturnMappedUser_WhenExists(int id)
    {
        //Arrange
        var user = new User { Id = id };
        var dto = new UserDto { Id = id };
        _userRepoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(dto);

        //Act
        var result = await _sut.GetByIdAsync(id);

        //Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetByIdAsync(-1)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetByIdAsync(-1);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnNull_WhenUserNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.DeleteAsync(-1)).ReturnsAsync((User)null);

        //Act
        var result = await _sut.DeleteAsync(-1);

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnDeletedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 1 };
        _userRepoMock.Setup(x => x.DeleteAsync(1)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 1 });

        //Act
        var result = await _sut.DeleteAsync(1);

        //Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnMappedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 1, Email = "test@example.com" };
        _userRepoMock.Setup(x => x.GetByEmailAsync(user.Email)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 1, Email = "test@example.com" });

        //Act
        var result = await _sut.GetByEmailAsync(user.Email);

        //Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
    }

    [Fact]
    public async Task GetByEmail_ShouldReturnNull_WhenNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetByEmailAsync("none")).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetByEmailAsync("none");

        //Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnMappedUser_WhenExists()
    {
        //Arrange
        var user = new User { Id = 2, Username = "ahmad" };
        _userRepoMock.Setup(x => x.GetByUsernameAsync(user.Username)).ReturnsAsync(user);
        _mapperMock.Setup(x => x.Map<UserDto>(user)).Returns(new UserDto { Id = 2, Username = "ahmad" });

        //Act
        var result = await _sut.GetByUsernameAsync(user.Username);

        //Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(user.Username);
    }

    [Fact]
    public async Task GetByUsername_ShouldReturnNull_WhenNotFound()
    {
        //Arrange
        _userRepoMock.Setup(x => x.GetByUsernameAsync("notfound")).ReturnsAsync((User)null);

        //Act
        var result = await _sut.GetByUsernameAsync("notfound");


        //Assert
        result.Should().BeNull();
    }
}