using System.ComponentModel.DataAnnotations;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Domain.Entities;

public class User
{
    public int Id { get; set; }
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    public UserRole Role { get; set; } = UserRole.Customer;
    [Required] public string Email { get; set; }
    public DateTime? BirthDate { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public IEnumerable<Review> Reviews { get; set; }
    public IEnumerable<Reservation> Reservations { get; set; }
    public User()
    {
        Reviews = new List<Review>();
        Reservations = new List<Reservation>();
    }
}