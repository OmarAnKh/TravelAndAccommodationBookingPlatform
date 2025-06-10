using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TravelAndAccommodationBookingPlatform.Domain.Entities;

namespace TravelAndAccommodationBookingPlatform.Domain.Interfaces;

public interface IAppDbContext
{
    DbSet<City> Cities { get; }
    DbSet<Location> Locations { get; }
    DbSet<Hotel> Hotels { get; }
    DbSet<Room> Rooms { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<Review> Reviews { get; }
    DbSet<User> Users { get; }
    EntityEntry Entry(object entity);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    void Dispose();
}