using Microsoft.AspNetCore.JsonPatch;
using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IService<T, in TParams, in TCreationDto, TUpdateDto, TDto> where T : class where TParams : IQueryParameters where TUpdateDto : class
{
    Task<(IEnumerable<TDto>, PaginationMetaData)> GetAllAsync(TParams queryParams);
    Task<TDto?> GetByIdAsync(int id);
    Task<TDto?> UpdateAsync(int id, JsonPatchDocument<TUpdateDto> patchDocument);
    Task<TDto?> DeleteAsync(int id);


}