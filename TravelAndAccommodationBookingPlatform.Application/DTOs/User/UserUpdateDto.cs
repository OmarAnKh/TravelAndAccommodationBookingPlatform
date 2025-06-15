using System.ComponentModel.DataAnnotations;

namespace TravelAndAccommodationBookingPlatform.Application.DTOs.User;

public class UserUpdateDto
{
    [Required] public int Id { get; set; }
    public string? Username { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
}