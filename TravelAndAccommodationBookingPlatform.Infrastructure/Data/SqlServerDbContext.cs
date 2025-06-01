using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Data;

public class SqlServerDbContext : DbContext
{
    public DbSet<City> Cities { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Hotel> Hotels { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        DotNetEnv.Env.Load();
        optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLSERVERCONNECTIONSTRING"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Hotel>()
            .HasOne(hotel => hotel.City)
            .WithMany(city => city.Hotels)
            .HasForeignKey(hotel => hotel.CityId);

        modelBuilder.Entity<Review>()
            .HasKey(r => new { r.UserId, r.HotelId });

        modelBuilder.Entity<Review>()
            .HasOne<Hotel>(r => r.Hotel)
            .WithMany(hotel => hotel.Reviews)
            .HasForeignKey(r => r.HotelId);


        modelBuilder.Entity<Review>()
            .HasOne<User>(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(hotel => hotel.Rooms)
            .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Reservation>()
            .HasKey(r => new { r.UserId, r.RoomId });

        modelBuilder.Entity<Reservation>()
            .HasOne<User>(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Reservation>()
            .HasOne<Room>(r => r.Room)
            .WithMany(r => r.Reservations)
            .HasForeignKey(r => r.RoomId);


        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Paris", Country = "France", Thumbnail = "paris.jpg", PostOffice = "75000", CreatedAt = DateTime.UtcNow },
            new City { Id = 2, Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", PostOffice = "100-0001", CreatedAt = DateTime.UtcNow },
            new City { Id = 3, Name = "New York", Country = "USA", Thumbnail = "nyc.jpg", PostOffice = "10001", CreatedAt = DateTime.UtcNow },
            new City { Id = 4, Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", PostOffice = "00100", CreatedAt = DateTime.UtcNow },
            new City { Id = 5, Name = "Barcelona", Country = "Spain", Thumbnail = "barcelona.jpg", PostOffice = "08001", CreatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Location>().HasData(
            new Location { HotelId = 1, Latitude = 48.8566f, Longitude = 2.3522f },
            new Location { HotelId = 2, Latitude = 35.6895f, Longitude = 139.6917f },
            new Location { HotelId = 3, Latitude = 40.7128f, Longitude = -74.0060f },
            new Location { HotelId = 4, Latitude = 41.9028f, Longitude = 12.4964f },
            new Location { HotelId = 5, Latitude = 41.3851f, Longitude = 2.1734f }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "alice", Password = "pass123", Email = "alice@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = 2, Username = "bob", Password = "pass123", Email = "bob@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = 3, Username = "carol", Password = "pass123", Email = "carol@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = 4, Username = "dave", Password = "pass123", Email = "dave@example.com", CreatedAt = DateTime.UtcNow },
            new User { Id = 5, Username = "eve", Password = "pass123", Email = "eve@example.com", CreatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Eiffel Hotel", CityId = 1, Description = "Near Eiffel Tower", Thumbnail = "eiffel_hotel.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Hotel { Id = 2, Name = "Shibuya Inn", CityId = 2, Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Hotel { Id = 3, Name = "Times Square Hotel", CityId = 3, Description = "Close to Broadway", Thumbnail = "ts_hotel.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Hotel { Id = 4, Name = "Colosseum Suites", CityId = 4, Description = "View of the Colosseum", Thumbnail = "colosseum.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new Hotel { Id = 5, Name = "Sagrada Familia Hotel", CityId = 5, Description = "Near Gaudi's masterpiece", Thumbnail = "sagrada.jpg", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        );

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, HotelId = 1, RoomType = RoomType.Single, Price = 120, Availability = "Available", CreatedAt = DateTime.UtcNow },
            new Room { Id = 2, HotelId = 2, RoomType = RoomType.Deluxe, Price = 200, Availability = "Available", CreatedAt = DateTime.UtcNow },
            new Room { Id = 3, HotelId = 3, RoomType = RoomType.Suite, Price = 300, Availability = "Available", CreatedAt = DateTime.UtcNow },
            new Room { Id = 4, HotelId = 4, RoomType = RoomType.Single, Price = 100, Availability = "Available", CreatedAt = DateTime.UtcNow },
            new Room { Id = 5, HotelId = 5, RoomType = RoomType.Deluxe, Price = 180, Availability = "Available", CreatedAt = DateTime.UtcNow }
        );

    }
}