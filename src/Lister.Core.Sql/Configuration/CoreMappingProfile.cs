using AutoMapper;
using Lister.Core.Entities.Views;

namespace Lister.Core.Sql.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinition>()
            .ReverseMap();
    }
}