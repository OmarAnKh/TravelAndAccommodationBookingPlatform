using TravelAndAccommodationBookingPlatform.Application.DTOs.User;
using TravelAndAccommodationBookingPlatform.Domain.Common.QueryParameters;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IUserService : IService<User, UserQueryParameters, UserCreationDto, UserUpdateDto, UserDto>
{
    Task<User?> GetById(int id);
    Task<User?> Delete(int id);
}