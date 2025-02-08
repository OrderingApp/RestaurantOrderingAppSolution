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

        CreateMap<(Table table, bool newOccupancy), TableOccupancyUpdatedEvent>()
            .ForMember(dest => dest.TableId, opt => opt.MapFrom(src => src.table.Id))
            .ForMember(dest => dest.NewOccupancy, opt => opt.MapFrom(src => src.newOccupancy));

    }
}
