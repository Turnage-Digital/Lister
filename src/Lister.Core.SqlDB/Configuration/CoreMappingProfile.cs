using AutoMapper;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListEntity, ListView>();

        CreateMap<ListEntity, IReadOnlyList>()
            .As<ListView>();

        CreateMap<StatusEntity, Status>();
        CreateMap<ColumnEntity, Column>();
    }
}