using AutoMapper;
using Domain;
using RestaurantOrdering.Events.Domain.Tables;

namespace Application.Core.EventsMapping.Tables;

public class TablesEventsMappingProfile : Profile
{
    public TablesEventsMappingProfile()
    {
        CreateMap<Table, TableCreatedEvent>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Table, TableUpdatedEvent>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id));

        CreateMap<Table, TableDeletedEvent>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.Id));

        CreateMap<(Table table, TableStatus previousTableStatus), TableStatusUpdatedEvent>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.table.Id))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.previousTableStatus))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.table.Status));
    }
}
