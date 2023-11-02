using AutoMapper;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Core.ValueObjects;

namespace Lister.Core.SqlDB.Configuration;

public class CoreMappingProfile : Profile
{
    public CoreMappingProfile()
    {
        CreateMap<ListDefEntity, ListDefView>();

        CreateMap<ListDefEntity, IReadOnlyListDef>()
            .As<ListDefView>();

        CreateMap<StatusDefEntity, StatusDef>();
        CreateMap<ColumnDefEntity, ColumnDef>();
    }
}