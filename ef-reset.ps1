# Rebuilds EF migrations and databases for all modules.
Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$configuration = "Debug"
$startupProject = "src/Lister.App.Server/Lister.App.Server.csproj"
$migrationName = "Initial"
$migrationOutputDir = "Migrations"

$modules = @(
    @{
        Name = "Lists"
        Project = "src/Modules/Lists/Lister.Lists.Infrastructure.Sql/Lister.Lists.Infrastructure.Sql.csproj"
        Context = "Lister.Lists.Infrastructure.Sql.ListsDbContext"
    },
    @{
        Name = "Users"
        Project = "src/Modules/Users/Lister.Users.Infrastructure.Sql/Lister.Users.Infrastructure.Sql.csproj"
        Context = "Lister.Users.Infrastructure.Sql.UsersDbContext"
    },
    @{
        Name = "Core"
        Project = "src/Modules/Core/Lister.Core.Infrastructure.Sql/Lister.Core.Infrastructure.Sql.csproj"
        Context = "Lister.Core.Infrastructure.Sql.CoreDbContext"
    },
    @{
        Name = "Notifications"
        Project = "src/Modules/Notifications/Lister.Notifications.Infrastructure.Sql/Lister.Notifications.Infrastructure.Sql.csproj"
        Context = "Lister.Notifications.Infrastructure.Sql.NotificationsDbContext"
    }
)

function Invoke-DotNetEf
{
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    Write-Host "dotnet ef $( $Arguments -join ' ' )" -ForegroundColor Cyan
    & dotnet ef @Arguments

    if ($LASTEXITCODE -ne 0)
    {
        throw "dotnet ef exited with code $LASTEXITCODE."
    }
}

function Get-EfCommonArgs
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Module
    )

    return @(
        "--project", $Module.Project,
        "--startup-project", $startupProject,
        "--context", $Module.Context,
        "--configuration", $configuration,
        "--verbose"
    )
}

function Reset-ModuleMigrations
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Module
    )

    $commonArgs = Get-EfCommonArgs -Module $Module

    $removeArgs = @("migrations", "remove") + $commonArgs + @("--force")
    Invoke-DotNetEf -Arguments $removeArgs

    $addArgs = @("migrations", "add", $migrationName) + $commonArgs + @("--output-dir", $migrationOutputDir)
    Invoke-DotNetEf -Arguments $addArgs
}

function Update-ModuleDatabase
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Module
    )

    $commonArgs = Get-EfCommonArgs -Module $Module

    $updateArgs = @("database", "update") + $commonArgs
    Invoke-DotNetEf -Arguments $updateArgs
}

function Remove-ModuleDatabase
{
    param(
        [Parameter(Mandatory = $true)]
        [hashtable]$Module
    )

    $commonArgs = Get-EfCommonArgs -Module $Module

    $dropArgs = @("database", "drop") + $commonArgs + @("--force")
    Invoke-DotNetEf -Arguments $dropArgs
}

$coreModule = $modules | Where-Object { $_.Name -eq "Core" }

if (-not $coreModule)
{
    throw "Core module configuration not found."
}

Remove-ModuleDatabase -Module $coreModule

foreach ($module in $modules)
{
    Reset-ModuleMigrations -Module $module
}

foreach ($module in $modules)
{
    Update-ModuleDatabase -Module $module
}
