using AutoMapper;
using Lister.Core.Views;

namespace Lister.Core.Sql.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinition>()
            .ReverseMap();
    }
}