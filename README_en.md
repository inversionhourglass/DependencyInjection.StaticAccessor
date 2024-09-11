# DependencyInjection.StaticAccessor

[中文](README.md) | English

`DependencyInjection.StaticAccessor` is dedicated to providing static access to the `IServiceProvider` object corresponding to the current DI Scope for various types of .NET projects. This allows you to easily use `IServiceProvider` in static methods and in types where direct interaction with DI services is not possible.

## NuGet Packages Overview

|                      Package Name                     |                                       Purpose                                       |
|:-----------------------------------------------------:|:------------------------------------------------------------------------------------|
| DependencyInjection.StaticAccessor.Hosting            | For AspNetCore projects (WebApi, Mvc, etc.) and Generic Host                        |
| DependencyInjection.StaticAccessor.Blazor             | For Blazor projects, including Blazor Server and Blazor WebAssembly Server          |
| DependencyInjection.StaticAccessor.Blazor.WebAssembly | For Blazor WebAssembly Client projects, also supports Auto mode for Client projects |
| DependencyInjection.StaticAccessor                    | Base library, usually not referenced directly                                       |

### Version Number Explanation

All version numbers follow the Semantic Versioning format. The major version matches the version of `Microsoft.Extensions.*` ( **please make sure the major version matches the `Microsoft.Extensions.*` version you are using when referencing the NuGet package**). The minor version indicates feature updates, while the patch version is for bug fixes.

## Quick Start

Install the corresponding NuGet package based on your project type, as listed in the [NuGet Packages Overview](#nuget-packages-overview).

```csharp
// 1. Initialization (Generic Host)
var builder = Host.CreateDefaultBuilder();

builder.UsePinnedScopeServiceProvider(); // Only this step to complete initialization

var host = builder.Build();

host.Run();

// 2. Access anywhere
class Test
{
    public static void M()
    {
        var yourService = PinnedScope.ScopedServices.GetService<IYourService>();
    }
}
```

The initialization method is similar for different project types, and all require calling the `UsePinnedScopeServiceProvider` extension method. Example initialization code for different project types will be provided later.

### AspNetCore Project Initialization Example

Install the NuGet package
> dotnet add package DependencyInjection.StaticAccessor.Hosting

```csharp
// 1. Initialization
var builder = WebApplication.CreateBuilder();

builder.Host.UsePinnedScopeServiceProvider(); // Just this step to complete the initialization

var app = builder.Build();

app.Run();
```

### Blazor Server-Side Project Initialization

**Note: The Blazor Server-side here includes Server, WebAssembly, and Auto modes of the Server-side project, not just the Server mode specifically.**

Install the NuGet package
> dotnet add package DependencyInjection.StaticAccessor.Blazor

The initialization process for Blazor Server-side is the same as AspNetCore. Please refer to the [AspNetCore Project Initialization Example](#aspnetcore-project-initialization-example). However, since Blazor's DI scope is different from the conventional one, additional steps are required.

#### Inherit PinnedScopeComponentBase for Pages

Due to Blazor's unique DI scope, all pages need to inherit from `PinnedScopeComponentBase`. It is recommended to define this once in `_Imports.razor` so that it applies to all pages.

```csharp
// _Imports.razor

@inherits DependencyInjection.StaticAccessor.Blazor.PinnedScopeComponentBase

```

In addition to `PinnedScopeComponentBase`, `PinnedScopeOwningComponentBase` and `PinnedScopeLayoutComponentBase` are also provided, and more types may be added as needed.

#### Solution for Existing Custom ComponentBase Base Classes

You might be using a `ComponentBase` base class defined by another package, and since C# does not support multiple inheritance, here is a solution that does not require inheriting from `PinnedScopeComponentBase`.

```csharp
// Assuming your current ComponentBase base class is ThirdPartyComponentBase

// Define a new base class that inherits from ThirdPartyComponentBase
public class YourComponentBase : ThirdPartyComponentBase, IHandleEvent, IServiceProviderHolder
{
    private IServiceProvider _serviceProvider;

    [Inject]
    public IServiceProvider ServiceProvider
    {
        get => _serviceProvider;
        set
        {
            PinnedScope.Scope = new FoolScope(value);
            _serviceProvider = value;
        }
    }

    Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
    {
        return this.PinnedScopeHandleEventAsync(callback, arg);
    }
}

// _Imports.razor
@inherits YourComponentBase
```

### Blazor WebAssembly Client Initialization

**Note: This is for Blazor WebAssembly Client-side initialization. For Server-side initialization, please refer to [Blazor Server-Side Project Initialization](#blazor-server-side-project-initialization)**

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.UsePinnedScopeServiceProvider(); // Just this step to complete the initialization

await builder.Build().RunAsync();
```

Similar to the Server-side, Client-side pages also need to inherit from `PinnedScopeComponentBase`. Please refer to [Inherit PinnedScopeComponentBase for Pages](#inherit-pinnedscopecomponentbase-for-pages).

## Notes

### Do Not Operate `IServiceScope` Through `PinnedScope`

Although you can access the current DI Scope via `PinnedScope.Scope`, please do not directly manipulate `PinnedScope.Scope`, such as calling the `Dispose` method. Instead, you should operate through the variable created when you initially created the scope.

### Does Not Support Non-Standard Scopes

In typical development scenarios, this issue usually does not need to be addressed, and it generally does not occur in standard AspNetCore projects. However, Blazor is an example of a non-standard DI Scope in official project types.

Before explaining what a non-standard Scope is, let's first talk about the standard Scope mode. We know that DI Scopes can be nested, and under normal circumstances, the nested Scopes form a stack structure, where the most recently created Scope is released first, in an orderly manner.

```csharp
using (var scope11 = serviceProvider.CreateScope())                    // push scope11. [scope11]
{
    using (var scope21 = scope11.ServiceProvider.CreateScope())        // push scope21. [scope11, scope21]
    {
        using (var scope31 = scope21.ServiceProvider.CreateScope())    // push scope31. [scope11, scope21, scope31]
        {

        }                                                              // pop scope31.  [scope11, scope21]

        using (var scope32 = scope21.ServiceProvider.CreateScope())    // push scope32. [scope11, scope21, scope32]
        {

        }                                                              // pop scope32.  [scope11, scope21]
    }                                                                  // pop scope21.  [scope11]

    using (var scope22 = scope11.ServiceProvider.CreateScope())        // push scope22. [scope11, scope22]
    {

    }                                                                  // pop scope22.  [scope11]
}                                                                      // pop scope11.  []
```

With this understanding of standard Scopes, non-standard Scopes can be easily understood. Any Scope that does not follow this orderly stack structure is considered a non-standard Scope. A common example is the scenario with Blazor:

As we know, Blazor SSR implements SPA through SignalR, where each SignalR connection corresponds to a DI Scope. Various events on the interface (clicks, focus acquisition, etc.) notify the server to callback event functions via SignalR. These callbacks are inserted externally to interact with SignalR. Without special handling, the Scope belonging to the callback event is a newly created Scope for that event. However, the `Component` we interact with in the callback event is created within the Scope belonging to SignalR, resulting in cross-Scope interaction. `PinnedScopeComponentBase` addresses this by resetting `PinnedScope.Scope` to the SignalR corresponding Scope before executing the callback function.
