using Application.Dtos.Allergens;
using Application.Dtos.Common;

namespace Application.Contracts;

public interface IAllergenService
{
    Task<ResultDto<AllergenReadDto>> CreateAllergen(AllergenCreateDto allergenCreateDto);
    Task<ResultDto<List<AllergenReadDto>>> GetAllAllergens();
    Task<ResultDto<AllergenReadDto>> GetAllergen(Guid id);
    Task<ResultDto<AllergenReadDto>> UpdateAllergen(AllergenUpdateDto allergenUpdateDto, Guid id);
    Task<ResultDto<bool>> DeleteAllergen(Guid id);
}
