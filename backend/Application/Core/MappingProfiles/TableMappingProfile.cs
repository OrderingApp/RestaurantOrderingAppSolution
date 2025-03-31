using Application.Dtos.Tables;
using AutoMapper;
using Domain;

namespace Application.Core.MappingProfiles;

public class TableMappingProfile : Profile
{
    public TableMappingProfile()
    {
        CreateMap<TableCreateDto, Table>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => TableStatus.Available))
            .ForMember(dest => dest.IsUsed, opt => opt.MapFrom(_ => true))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.AreaId, opt => opt.MapFrom(src => src.AreaId));

        CreateMap<Table, TableReadDto>()
            .ForMember(
                dest => dest.Reservation,
                opt => opt.MapFrom(src => src.Reservations.FirstOrDefault(r => !r.IsAssigned))
            );

        CreateMap<Table, TableSummaryDto>()
            .ForMember(dest => dest.Orders, opt => opt.MapFrom(src => src.Orders));

        CreateMap<TableUpdateDto, Table>()
            .ForMember(dest => dest.Status, opt => opt.Condition(src => src.Status.HasValue));
    }
}
