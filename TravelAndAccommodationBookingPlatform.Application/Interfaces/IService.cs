using TravelAndAccommodationBookingPlatform.Domain.Common;

namespace TravelAndAccommodationBookingPlatform.Application.Interfaces;

public interface IService<T, in TParams, in TCreationDto, in TUpdateDto, TDto> where T : class where TParams : IQueryParameters
{
    Task<(IEnumerable<TDto>, PaginationMetaData)> GetAll(TParams queryParams);
    Task<TDto?> Create(TCreationDto entity);
    Task<TDto?> UpdateAsync(TUpdateDto entity);
}