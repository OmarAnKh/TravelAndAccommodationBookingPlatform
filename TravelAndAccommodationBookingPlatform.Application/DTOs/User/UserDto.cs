using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.User;

public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public UserRole Role { get; set; }
    public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}