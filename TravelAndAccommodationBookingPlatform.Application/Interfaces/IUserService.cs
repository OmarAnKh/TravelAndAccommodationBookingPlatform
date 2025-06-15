using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IUserService : IService<User, UserQueryParameters, UserCreationDto, UserUpdateDto, UserDto>
{
    Task<UserDto?> GetById(int id);
    Task<UserDto?> Delete(int id);
    Task<UserDto?> GetByEmail(string email);
    Task<UserDto?> GetByUsername(string username);
}