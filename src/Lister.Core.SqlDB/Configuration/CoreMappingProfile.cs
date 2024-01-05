using AutoMapper;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;

namespace Lister.Core.SqlDB.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListEntity, ListView>()
            .ReverseMap();

        CreateMap<ListEntity, IReadOnlyList>()
            .As<ListView>();
    }
}