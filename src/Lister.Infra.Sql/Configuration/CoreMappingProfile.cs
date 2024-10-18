using AutoMapper;
using Lister.Domain.Views;
using Lister.Infra.Sql.Entities;

namespace Lister.Infra.Sql.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinition>()
            .ReverseMap();
    }
}