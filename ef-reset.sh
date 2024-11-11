/usr/local/share/dotnet/dotnet ef database drop --project src/Modules/Core/Lister.Core.Infrastructure.Sql/Lister.Core.Infrastructure.Sql.csproj --startup-project src/Lister.Server/Lister.Server.csproj --context Lister.Core.Infrastructure.Sql.DataProtectionKeyDbContext --configuration Debug --verbose --force

/usr/local/share/dotnet/dotnet ef migrations remove --project src/Modules/Lists/Lister.Lists.Infrastructure.Sql/Lister.Lists.Infrastructure.Sql.csproj --startup-project src/Lister.Server/Lister.Server.csproj --context Lister.Lists.Infrastructure.Sql.ListsDbContext --configuration Debug --verbose --force

/usr/local/share/dotnet/dotnet ef migrations add --project src/Modules/Lists/Lister.Lists.Infrastructure.Sql/Lister.Lists.Infrastructure.Sql.csproj --startup-project src/Lister.Server/Lister.Server.csproj --context Lister.Lists.Infrastructure.Sql.ListsDbContext --configuration Debug --verbose Initial --output-dir Migrations


