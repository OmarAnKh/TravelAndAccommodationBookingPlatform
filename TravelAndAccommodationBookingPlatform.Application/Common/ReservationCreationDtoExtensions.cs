using TravelAndAccommodationBookingPlatform.Application.DTOs.Reservation;

namespace TravelAndAccommodationBookingPlatform.Application.Common;

public static class ReservationCreationDtoExtensions
{
    public static Dictionary<string, string> FromReservationCreationDtoToMetadata(this ReservationCreationDto dto, string userId)
    {
        return new Dictionary<string, string>
        {
            { "userId", userId },
            { "roomId", dto.RoomId.ToString() },
            { "startDate", dto.StartDate.ToString("o") },
            { "endDate", dto.EndDate.ToString("o") },
            { "bookPrice", dto.BookPrice.ToString("F2") },
        };
    }
    public static ReservationCreationDto FromMetadataReservationCreationDto(this Dictionary<string, string> metadata)
    {
        return new ReservationCreationDto
        {
            RoomId = int.Parse(metadata["roomId"]),
            StartDate = DateTime.Parse(metadata["startDate"]),
            EndDate = DateTime.Parse(metadata["endDate"]),
            BookPrice = float.Parse(metadata["bookPrice"]),
        };
    }
}