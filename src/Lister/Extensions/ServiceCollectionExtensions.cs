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

namespace Lister.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        var domainAssemblyName = typeof(ListDefAggregate<>).Assembly;
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(domainAssemblyName));
        services.AddSingleton<ListDefAggregate<ListDefEntity>>();
        return services;
    }

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var applicationAssemblyName = typeof(CreateListDefCommand<>).Assembly;
        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(applicationAssemblyName));
        services.AddTransient<IRequestHandler<CreateListDefCommand<ListDefView>, ListDefView>,
            CreateListDefCommandHandler<ListDefView, ListDefEntity>>();
        services.AddTransient<IRequestHandler<GetListDefByIdQuery<ListDefView>, ListDefView>,
            GetListDefByIdQueryHandler<ListDefView>>();
        services.AddTransient<IRequestHandler<GetListDefsQuery<ListDefView>, ListDefView[]>,
            GetThingDefsQueryHandler<ListDefView>>();
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
                optionsBuilder => optionsBuilder.MigrationsAssembly(migrationAssemblyName)));
        services.AddScoped<IListerUnitOfWork<ListDefEntity>, ListerUnitOfWork>();
        services.AddStores();
        services.AddViews();
        services.AddAutoMapper(config =>
            config.AddProfile<CoreMappingProfile>());
        return services;
    }

    private static IServiceCollection AddStores(this IServiceCollection services)
    {
        services.AddScoped<IListDefsStore<ListDefEntity>, ListDefsStore>();
        return services;
    }

    private static IServiceCollection AddViews(this IServiceCollection services)
    {
        services.AddScoped<IGetReadOnlyListDefById<ListDefView>, GetReadOnlyListDefById>();
        services.AddScoped<IGetReadOnlyListDefs<ListDefView>, GetReadOnlyListDefs>();
        return services;
    }
}