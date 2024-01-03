using Lister.Application.Commands;
using Lister.Application.Queries;
using Lister.Core;
using Lister.Core.SqlDB;
using Lister.Core.SqlDB.Configuration;
using Lister.Core.SqlDB.Entities;
using Lister.Core.SqlDB.Views;
using Lister.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Lister.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        var domainAssemblyName = typeof(ListAggregate<>).Assembly;
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(domainAssemblyName));
        services.AddSingleton<ListAggregate<ListEntity>>();
        return services;
    }
    
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssemblyName = typeof(CreateListCommand<>).Assembly;
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(applicationAssemblyName));
        services.AddTransient<IRequestHandler<CreateListCommand<ListView>, ListView>,
            CreateListCommandHandler<ListView, ListEntity>>();
        services.AddTransient<IRequestHandler<GetListByIdQuery<ListView>, ListView>,
            GetListByIdQueryHandler<ListView>>();
        services.AddTransient<IRequestHandler<GetListsQuery<ListView>, ListView[]>,
            GetListsQueryHandler<ListView>>();
        services.AddTransient<IRequestHandler<GetListNamesQuery<ListNameView>, ListNameView[]>,
            GetListNamesQueryHandler<ListNameView>>();
        return services;
    }

    public static IServiceCollection AddCore(this IServiceCollection services, string connectionString)
    {
        var migrationAssemblyName = typeof(ListerDbContext).Assembly.FullName!;
        var serverVersion = ServerVersion.AutoDetect(connectionString);
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, serverVersion,
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddDbContext<ListerDbContext>(options =>
            options.UseMySql(connectionString, serverVersion,
                    optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName))
                .ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning)));
        services.AddScoped<IListerUnitOfWork<ListEntity>, ListerUnitOfWork>();
        services.AddStores();
        services.AddViews();
        services.AddAutoMapper(config =>
            config.AddProfile<CoreMappingProfile>());
        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        services.AddScoped<IListsStore<ListEntity>, ListsStore>();
        return services;
    }

    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddScoped<IGetListById<ListView>, GetListById>();
        services.AddScoped<IGetListItemDefinitionById<ListItemDefinitionView>, GetListItemDefinitionById>();
        services.AddScoped<IGetListNames<ListNameView>, GetListNames>();
        services.AddScoped<IGetLists<ListView>, GetLists>();
        return services;
    }
}