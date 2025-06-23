using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Data;

public interface IAppDbContext
{
    DbSet<City> Cities { get; set; }
    DbSet<Location> Locations { get; set; }
    DbSet<Hotel> Hotels { get; set; }
    DbSet<Room> Rooms { get; set; }
    DbSet<Reservation> Reservations { get; set; }
    DbSet<Review> Reviews { get; set; }
    DbSet<User> Users { get; set; }
    DbSet<RefreshToken> RefreshTokens { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void Dispose();
}