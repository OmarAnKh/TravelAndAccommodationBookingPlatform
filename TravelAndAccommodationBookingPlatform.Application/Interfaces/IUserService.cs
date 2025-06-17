using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IUserService : IService<User, UserQueryParameters, UserCreationDto, UserUpdateDto, UserDto>
{
    Task<UserDto?> GetByIdAsync(int id);
    Task<UserDto?> DeleteAsync(int id);
    Task<UserDto?> GetByEmailAsync(string email);
    Task<UserDto?> GetByUsernameAsync(string username);
    Task<UserDto?> CreateAsync(UserCreationDto entity);
    Task<UserDto?> UpdateAsync(int id, JsonPatchDocument<UserUpdateDto> patchDocument);


}