using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<(IEnumerable<UserDto>, PaginationMetaData)> GetAllAsync(UserQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _userRepository.GetAllAsync(queryParams);
        var users = _mapper.Map<IEnumerable<UserDto>>(entities);
        return (users, paginationMetaData);
    }

    public async Task<UserDto?> CreateAsync(UserCreationDto entity)
    {
        var isNotValid = await UserExists(entity);
        if (isNotValid)
        {
            return null;
        }
        var user = _mapper.Map<User>(entity);
        user.Password = BCrypt.Net.BCrypt.HashPassword(entity.Password);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        var createdUser = await _userRepository.CreateAsync(user);
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(createdUser);
    }
    public async Task<UserDto?> UpdateAsync(int id, JsonPatchDocument<UserUpdateDto> patchDocument)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return null;
        }

        var updatedUser = _mapper.Map<UserUpdateDto>(user);
        patchDocument.ApplyTo(updatedUser);

        _mapper.Map(updatedUser, user);

        var isUsernameTaken = await IsUsernameUsed(user.Username, id);
        if (isUsernameTaken)
        {
            return null;
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.SaveChangesAsync();
        return _mapper.Map<UserDto>(user);
    }


    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }


    public async Task<UserDto?> DeleteAsync(int id)
    {
        var deleteResult = await _userRepository.DeleteAsync(id);
        if (deleteResult is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(deleteResult);
    }
    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> ValidateCredentialsAsync(string email, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(email);
        if (user is null)
        {
            return null;
        }
        bool isValidPassword = BCrypt.Net.BCrypt.Verify(password, user.Password);
        return isValidPassword ? _mapper.Map<UserDto>(user) : null;
    }
    public async Task<UserDto?> UpdateRoleAsync(int userId, UserRole newRole)
    {
        var user = await _userRepository.UpdateUserRoleAsync(userId, newRole);
        if (user == null)
            return null;

        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }
    public async Task<UserDto?> GetUserByUsernameAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }

    private async Task<bool> UserExists(UserCreationDto entity)
    {
        var usernameExists = await _userRepository.GetByUsernameAsync(entity.Username);
        var emailExists = await _userRepository.GetByEmailAsync(entity.Email);
        if (usernameExists != null || emailExists != null)
        {
            return true;
        }
        return false;
    }

    private async Task<bool> IsUsernameUsed(string username, int excludeUserId)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return user != null && user.Id != excludeUserId;
    }
}