using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Application.Interfaces;
using TravelAndAccommodationBookingPlatform.Domain.Common;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
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
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        var createdUser = await _userRepository.CreateAsync(user);
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
}