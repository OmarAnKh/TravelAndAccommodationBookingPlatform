namespace TravelAndAccommodationBookingPlatform.Application.DTOs.User;

public class UserUpdateDto
{
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}