using AutoMapper;
using Lister.Domain.Views;
using Lister.Infrastructure.Sql.Entities;

namespace Lister.Infrastructure.Sql.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDb, ListItemDefinition>()
            .ReverseMap();
    }
}