using Microsoft.EntityFrameworkCore;
using TravelAndAccommodationBookingPlatform.Domain.Entities;
using TravelAndAccommodationBookingPlatform.Domain.Enums;
using TravelAndAccommodationBookingPlatform.Domain.Interfaces;

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Data;

public class SqlServerDbContext : DbContext, IAppDbContext
{
    public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options) : base(options)
    {
    }

    public SqlServerDbContext()
    {
    }

    public virtual DbSet<City> Cities { get; set; }
    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<Hotel> Hotels { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<Reservation> Reservations { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            DotNetEnv.Env.Load();
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQLSERVERCONNECTIONSTRING"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Hotel-City relationship
        modelBuilder.Entity<Hotel>()
            .HasOne(hotel => hotel.City)
            .WithMany(city => city.Hotels)
            .HasForeignKey(hotel => hotel.CityId);

        // Review composite key and relationships
        modelBuilder.Entity<Review>()
            .HasKey(r => r.ReviewId);

        modelBuilder.Entity<Review>()
            .HasOne<Hotel>(r => r.Hotel)
            .WithMany(hotel => hotel.Reviews)
            .HasForeignKey(r => r.HotelId);

        modelBuilder.Entity<Review>()
            .HasOne<User>(r => r.User)
            .WithMany(u => u.Reviews)
            .HasForeignKey(r => r.UserId);

        // Room-Hotel relationship
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Hotel)
            .WithMany(hotel => hotel.Rooms)
            .HasForeignKey(r => r.HotelId);

        // Reservation composite key and relationships
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

        // Seed data - only add if not in testing environment
        if (!Database.ProviderName.Contains("InMemory"))
        {
            SeedData(modelBuilder);
        }
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        var baseDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<City>().HasData(
            new City { Id = 1, Name = "Paris", Country = "France", Thumbnail = "paris.jpg", PostOffice = "75000", CreatedAt = baseDate },
            new City { Id = 2, Name = "Tokyo", Country = "Japan", Thumbnail = "tokyo.jpg", PostOffice = "100-0001", CreatedAt = baseDate },
            new City { Id = 3, Name = "New York", Country = "USA", Thumbnail = "nyc.jpg", PostOffice = "10001", CreatedAt = baseDate },
            new City { Id = 4, Name = "Rome", Country = "Italy", Thumbnail = "rome.jpg", PostOffice = "00100", CreatedAt = baseDate },
            new City { Id = 5, Name = "Barcelona", Country = "Spain", Thumbnail = "barcelona.jpg", PostOffice = "08001", CreatedAt = baseDate }
        );

        modelBuilder.Entity<Location>().HasData(
            new Location { HotelId = 1, Latitude = 48.8566f, Longitude = 2.3522f },
            new Location { HotelId = 2, Latitude = 35.6895f, Longitude = 139.6917f },
            new Location { HotelId = 3, Latitude = 40.7128f, Longitude = -74.0060f },
            new Location { HotelId = 4, Latitude = 41.9028f, Longitude = 12.4964f },
            new Location { HotelId = 5, Latitude = 41.3851f, Longitude = 2.1734f }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Username = "alice", Password = "pass123", Email = "alice@example.com", CreatedAt = baseDate },
            new User { Id = 2, Username = "bob", Password = "pass123", Email = "bob@example.com", CreatedAt = baseDate },
            new User { Id = 3, Username = "carol", Password = "pass123", Email = "carol@example.com", CreatedAt = baseDate },
            new User { Id = 4, Username = "dave", Password = "pass123", Email = "dave@example.com", CreatedAt = baseDate },
            new User { Id = 5, Username = "eve", Password = "pass123", Email = "eve@example.com", CreatedAt = baseDate }
        );

        modelBuilder.Entity<Hotel>().HasData(
            new Hotel { Id = 1, Name = "Eiffel Hotel", CityId = 1, Owner = "Anan Khalili", Description = "Near Eiffel Tower", Thumbnail = "eiffel_hotel.jpg", CreatedAt = baseDate, UpdatedAt = baseDate },
            new Hotel { Id = 2, Name = "Shibuya Inn", CityId = 2, Owner = "Idk", Description = "In the heart of Tokyo", Thumbnail = "shibuya_inn.jpg", CreatedAt = baseDate, UpdatedAt = baseDate },
            new Hotel { Id = 3, Name = "Times Square Hotel", CityId = 3, Owner = "Ahmad", Description = "Close to Broadway", Thumbnail = "ts_hotel.jpg", CreatedAt = baseDate, UpdatedAt = baseDate },
            new Hotel { Id = 4, Name = "Colosseum Suites", CityId = 4, Owner = "Rahaf", Description = "View of the Colosseum", Thumbnail = "colosseum.jpg", CreatedAt = baseDate, UpdatedAt = baseDate },
            new Hotel { Id = 5, Name = "Sagrada Familia Hotel", CityId = 5, Owner = "YOU", Description = "Near Gaudi's masterpiece", Thumbnail = "sagrada.jpg", CreatedAt = baseDate, UpdatedAt = baseDate }
        );

        modelBuilder.Entity<Room>().HasData(
            new Room { Id = 1, HotelId = 1, RoomType = RoomType.Single, Price = 120, Availability = Availability.Available, CreatedAt = baseDate },
            new Room { Id = 2, HotelId = 2, RoomType = RoomType.Deluxe, Price = 200, Availability = Availability.Unavailable, CreatedAt = baseDate },
            new Room { Id = 3, HotelId = 3, RoomType = RoomType.Suite, Price = 300, Availability = Availability.Available, CreatedAt = baseDate },
            new Room { Id = 4, HotelId = 4, RoomType = RoomType.Single, Price = 100, Availability = Availability.Unavailable, CreatedAt = baseDate },
            new Room { Id = 5, HotelId = 5, RoomType = RoomType.Deluxe, Price = 180, Availability = Availability.Available, CreatedAt = baseDate }
        );

        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                ReviewId = 1,
                UserId = 1,
                HotelId = 1,
                Comment = "Amazing service and beautiful view!",
                Rate = 4.8f, ImagePath = "images/reviews/review1.jpg",
                CreatedAt = new DateTime(2024, 12, 15),
                UpdatedAt = new DateTime(2024, 12, 15)
            },
            new Review
            {
                ReviewId = 2,
                UserId = 2,
                HotelId = 1,
                Comment = "Good location but noisy at night.",
                Rate = 3.5f,
                ImagePath = "images/reviews/review2.jpg",
                CreatedAt = new DateTime(2025, 1, 10),
                UpdatedAt = new DateTime(2025, 1, 10)
            },
            new Review
            {
                ReviewId = 3,
                UserId = 1,
                HotelId = 2,
                Comment = "Clean rooms and friendly staff.",
                Rate = 4.2f,
                ImagePath = null,
                CreatedAt = new DateTime(2025, 2, 20),
                UpdatedAt = new DateTime(2025, 2, 21)
            },
            new Review
            {
                ReviewId = 4,
                UserId = 3,
                HotelId = 2,
                Comment = "Mediocre experience overall.",
                Rate = 2.9f,
                ImagePath = "images/reviews/review4.jpg",
                CreatedAt = new DateTime(2025, 3, 5),
                UpdatedAt = new DateTime(2025, 3, 5)
            },
            new Review
            {
                ReviewId = 5,
                UserId = 2,
                HotelId = 3,
                Comment = "Best stay I’ve had in years!",
                Rate = 5.0f,
                ImagePath = "images/reviews/review5.jpg",
                CreatedAt = new DateTime(2025, 4, 18),
                UpdatedAt = new DateTime(2025, 4, 18)
            }
        );

    }
}