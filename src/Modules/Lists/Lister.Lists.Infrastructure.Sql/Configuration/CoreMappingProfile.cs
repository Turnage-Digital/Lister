using AutoMapper;
using Lister.Lists.Domain.Views;
using Lister.Lists.Infrastructure.Sql.Entities;

namespace Lister.Lists.Infrastructure.Sql.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinition>()
            .ReverseMap();
    }
}