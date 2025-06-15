using AutoMapper;
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

    public async Task<(IEnumerable<UserDto>, PaginationMetaData)> GetAll(UserQueryParameters queryParams)
    {
        var (entities, paginationMetaData) = await _userRepository.GetAll(queryParams);
        var users = _mapper.Map<IEnumerable<UserDto>>(entities);
        return (users, paginationMetaData);
    }

    public async Task<UserDto?> Create(UserCreationDto entity)
    {
        var isNotValid = await UserExists(entity);
        if (isNotValid)
        {
            return null;
        }
        var user = _mapper.Map<User>(entity);
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;
        var createdUser = await _userRepository.Create(user);
        return _mapper.Map<UserDto>(createdUser);
    }
    public async Task<UserDto?> UpdateAsync(UserUpdateDto entity)
    {
        var usernameExists = await _userRepository.GetByUsername(entity.Username);
        if (usernameExists is not null)
        {
            return null;
        }
        var user = _mapper.Map<User>(entity);
        user.UpdatedAt = DateTime.UtcNow;
        var updateResult = await _userRepository.UpdateAsync(user);
        if (updateResult is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(updateResult);

    }

    public async Task<UserDto?> GetById(int id)
    {
        var user = await _userRepository.GetById(id);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }


    public async Task<UserDto?> Delete(int id)
    {
        var deleteResult = await _userRepository.Delete(id);
        if (deleteResult is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(deleteResult);
    }
    public async Task<UserDto?> GetByEmail(string email)
    {
        var user = await _userRepository.GetByEmail(email);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto?> GetByUsername(string username)
    {
        var user = await _userRepository.GetByUsername(username);
        if (user is null)
        {
            return null;
        }
        return _mapper.Map<UserDto>(user);
    }

    private async Task<bool> UserExists(UserCreationDto entity)
    {
        var usernameExists = await _userRepository.GetByUsername(entity.Username);
        var emailExists = await _userRepository.GetByEmail(entity.Email);
        if (usernameExists != null || emailExists != null)
        {
            return true;
        }
        return false;
    }
}