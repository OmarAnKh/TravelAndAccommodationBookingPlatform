﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TravelAndAccommodationBookingPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Owner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hotels_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    Longitude = table.Column<float>(type: "real", nullable: false),
                    Latitude = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.HotelId);
                    table.ForeignKey(
                        name: "FK_Locations_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Rate = table.Column<float>(type: "real", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => new { x.UserId, x.HotelId });
                    table.ForeignKey(
                        name: "FK_Reviews_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HotelId = table.Column<int>(type: "int", nullable: false),
                    RoomType = table.Column<int>(type: "int", nullable: false),
                    CustomRoomTypeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<float>(type: "real", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Availability = table.Column<int>(type: "int", nullable: false),
                    Adults = table.Column<int>(type: "int", nullable: false),
                    Children = table.Column<int>(type: "int", nullable: false),
                    RoomNumber = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_HotelId",
                        column: x => x.HotelId,
                        principalTable: "Hotels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookPrice = table.Column<float>(type: "real", nullable: false),
                    BookDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatus = table.Column<int>(type: "int", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => new { x.UserId, x.RoomId });
                    table.ForeignKey(
                        name: "FK_Reservations_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "CreatedAt", "Name", "PostOffice", "Thumbnail", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "France", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(284), "Paris", "75000", "paris.jpg", null },
                    { 2, "Japan", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(288), "Tokyo", "100-0001", "tokyo.jpg", null },
                    { 3, "USA", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(290), "New York", "10001", "nyc.jpg", null },
                    { 4, "Italy", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(292), "Rome", "00100", "rome.jpg", null },
                    { 5, "Spain", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(293), "Barcelona", "08001", "barcelona.jpg", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedAt", "Email", "Password", "PhoneNumber", "Role", "Username" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(455), "alice@example.com", "pass123", null, 1, "alice" },
                    { 2, null, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(457), "bob@example.com", "pass123", null, 1, "bob" },
                    { 3, null, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(459), "carol@example.com", "pass123", null, 1, "carol" },
                    { 4, null, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(460), "dave@example.com", "pass123", null, 1, "dave" },
                    { 5, null, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(461), "eve@example.com", "pass123", null, 1, "eve" }
                });

            migrationBuilder.InsertData(
                table: "Hotels",
                columns: new[] { "Id", "CityId", "CreatedAt", "Description", "Name", "Owner", "Thumbnail", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(482), "Near Eiffel Tower", "Eiffel Hotel", "Anan Khalili", "eiffel_hotel.jpg", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(482) },
                    { 2, 2, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(484), "In the heart of Tokyo", "Shibuya Inn", "Idk", "shibuya_inn.jpg", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(485) },
                    { 3, 3, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(486), "Close to Broadway", "Times Square Hotel", "Ahmad", "ts_hotel.jpg", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(487) },
                    { 4, 4, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(488), "View of the Colosseum", "Colosseum Suites", "Rahaf", "colosseum.jpg", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(488) },
                    { 5, 5, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(490), "Near Gaudi's masterpiece", "Sagrada Familia Hotel", "YOU", "sagrada.jpg", new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(491) }
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "HotelId", "Latitude", "Longitude" },
                values: new object[,]
                {
                    { 1, 48.8566f, 2.3522f },
                    { 2, 35.6895f, 139.6917f },
                    { 3, 40.7128f, -74.006f },
                    { 4, 41.9028f, 12.4964f },
                    { 5, 41.3851f, 2.1734f }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "Id", "Adults", "Availability", "Children", "CreatedAt", "CustomRoomTypeName", "Description", "HotelId", "Price", "RoomNumber", "RoomType", "Thumbnail", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 0, 1, 0, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(512), null, null, 1, 120f, null, 1, null, null },
                    { 2, 0, 2, 0, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(514), null, null, 2, 200f, null, 6, null, null },
                    { 3, 0, 1, 0, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(516), null, null, 3, 300f, null, 4, null, null },
                    { 4, 0, 2, 0, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(517), null, null, 4, 100f, null, 1, null, null },
                    { 5, 0, 1, 0, new DateTime(2025, 6, 1, 13, 16, 34, 593, DateTimeKind.Utc).AddTicks(518), null, null, 5, 180f, null, 6, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hotels_CityId",
                table: "Hotels",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_HotelId",
                table: "Reviews",
                column: "HotelId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_HotelId",
                table: "Rooms",
                column: "HotelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Hotels");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
