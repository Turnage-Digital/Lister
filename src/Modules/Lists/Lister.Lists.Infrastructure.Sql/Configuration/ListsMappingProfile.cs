using AutoMapper;
using Lister.Lists.ReadOnly.Dtos;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.Configuration;

public class ListsMappingProfile : Profile
{
    public ListsMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinitionDto>()
            .ReverseMap();
        CreateMap<ItemDb, ListItemDto>()
            .ReverseMap();
        CreateMap<ItemDb, ItemDetailsDto>()
            .ReverseMap();
    }
}
