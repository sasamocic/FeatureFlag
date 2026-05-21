# FeatureFlag

Lightweight .NET feature toggles backed by configuration. Read boolean flags from the `FeatureFlags` section and check them through `IFeatureFlags.IsEnabled`.

## Install

```bash
dotnet add package FeatureFlag
```

Or add a `PackageReference` in your project file:

```xml
<PackageReference Include="FeatureFlag" Version="1.1.0" />
```

## Configuration

Define flags under the `FeatureFlags` section in `appsettings.json` (or environment variables, user secrets, etc.):

```json
{
  "FeatureFlags": {
    "NewCheckout": true,
    "BetaDashboard": false
  }
}
```

Environment variable override (double underscore maps to nested keys):

```bash
FeatureFlags__NewCheckout=true
```

Nested flags use colon notation in code (maps to nested JSON):

```json
{
  "FeatureFlags": {
    "Experiments": {
      "DarkMode": true
    }
  }
}
```

Check with `IsEnabled("Experiments:DarkMode")`.

| Value | Result |
|-------|--------|
| `true` / `false` | Enabled or disabled |
| Missing key | `false` |
| Non-boolean (`"1"`, `"maybe"`, empty) | `false` |
| `null`, empty, or whitespace name | `false` |

Flag names are matched case-insensitively when using JSON configuration.

## ASP.NET Core

Register in `Program.cs`:

```csharp
using FeatureFlag;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFeatureFlags(builder.Configuration);

var app = builder.Build();
```

Inject `IFeatureFlags` where you need branching logic:

```csharp
public class CheckoutController : ControllerBase
{
    private readonly IFeatureFlags _featureFlags;

    public CheckoutController(IFeatureFlags featureFlags)
    {
        _featureFlags = featureFlags;
    }

    [HttpGet]
    public IActionResult Get()
    {
        if (_featureFlags.IsEnabled("NewCheckout"))
            return Ok(/* new flow */);

        return Ok(/* legacy flow */);
    }
}
```

## Generic host / worker

```csharp
using FeatureFlag;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddFeatureFlags(context.Configuration);
        services.AddHostedService<MyWorker>();
    })
    .Build();

await host.RunAsync();

public sealed class MyWorker : BackgroundService
{
    private readonly IFeatureFlags _featureFlags;

    public MyWorker(IFeatureFlags featureFlags) => _featureFlags = featureFlags;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_featureFlags.IsEnabled("BetaDashboard"))
            {
                // beta path
            }

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
```

## Without DI

You can construct the implementation directly when you only have `IConfiguration`:

```csharp
using FeatureFlag;
using Microsoft.Extensions.Configuration;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

IFeatureFlags flags = new ConfigurationFeatureFlags(configuration);

if (flags.IsEnabled("NewCheckout"))
{
    Console.WriteLine("New checkout is on.");
}
```

## API

| Member | Description |
|--------|-------------|
| `IFeatureFlags.IsEnabled(string featureName)` | Returns whether the flag is enabled |
| `IFeatureFlags.SectionName` | Configuration section name (`FeatureFlags`) |
| `ServiceCollectionExtensions.AddFeatureFlags(...)` | Registers `IFeatureFlags` as a singleton |
| `ConfigurationFeatureFlags.DefaultSectionName` | Constant `"FeatureFlags"` |

## Requirements

- .NET 8.0
- `Microsoft.Extensions.Configuration`
- `Microsoft.Extensions.DependencyInjection` (for `AddFeatureFlags`)

## License

See repository for license details.
