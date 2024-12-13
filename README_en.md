# DependencyInjection.StaticAccessor

[中文](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/README.md) | English

`DependencyInjection.StaticAccessor` is dedicated to providing static access to the `IServiceProvider` object corresponding to the current DI Scope for various types of .NET projects. This allows you to easily use `IServiceProvider` in static methods and in types where direct interaction with DI services is not possible.

## Why Do We Need PinnedScope

When working with dependency injection, a common challenge is accessing the IoC/DI container within static methods. There are many solutions to this problem, and the simplest is to save the root container to a static variable during application startup and access it directly afterward:

```csharp
public static IServiceProvider ServiceProvider;

static void Main(string[] args)
{
    var builder = Host.CreateDefaultBuilder();

    var host = builder.Build();

    ServiceProvider = host.Services;

    host.Run();
}
```

This approach is straightforward and effective for simple scenarios. However, we know that Microsoft Dependency Injection offers three service lifetimes: `Transient`, `Scoped`, and `Singleton`. Among them, `Scoped` is particularly special. In web development, a `Scoped` service corresponds to the lifetime of a single request. For services registered with the `Scoped` lifetime, we cannot retrieve them directly from the root container; they must be accessed through a scope-specific container. 

For this requirement in web applications, there is an easy solution. Microsoft provides the `IHttpContextAccessor`, which allows us to obtain the current request's `IServiceProvider` via `IHttpContextAccessor.HttpContext.RequestServices` after retrieving the `IHttpContextAccessor` object from the root container.

### Why Is PinnedScope Necessary?

Despite the existence of these straightforward solutions, scope management becomes more complex in advanced scenarios. For instance, in web applications, you might need to launch a background thread to complete some asynchronous operations, allowing the request to proceed without waiting for the operation to finish. In such asynchronous operations, DI containers are often required. How can we access the DI container in a static method in this case? Can we continue using the root container or `IHttpContextAccessor`? The answer is no, because by this point, the request may have already been completed, and accessing `IServiceProvider` through `IHttpContextAccessor` will result in an exception, as it may reference a disposed service provider.

In such cases, you usually need to create a new scope specifically for the asynchronous operation. While you can manage these scopes manually, `PinnedScope` offers a simpler alternative.

The above is just one example. In reality, scope management can be even more complicated in scenarios like task scheduling, message subscriptions, and even Microsoft's official Blazor framework. While you could find tailored solutions for each scenario to access the `IServiceProvider`, `PinnedScope` provides a unified and simple approach. With `PinnedScope`, you don't need to worry about how your framework creates and manages scopes—you can always retrieve the correct `IServiceProvider` effortlessly.

## NuGet Packages Overview

|                      Package Name                     |                                       Purpose                                       |
|:-----------------------------------------------------:|:------------------------------------------------------------------------------------|
| DependencyInjection.StaticAccessor.Hosting            | For AspNetCore projects (WebApi, Mvc, etc.) and Generic Host                        |
| DependencyInjection.StaticAccessor.Blazor             | For Blazor projects, including Blazor Server and Blazor WebAssembly Server          |
| DependencyInjection.StaticAccessor.Blazor.WebAssembly | For Blazor WebAssembly Client projects, also supports Auto mode for Client projects |
| DependencyInjection.StaticAccessor                    | Base library, A non-startup project using PinnedScope references the package        |

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

**Note: The Blazor Server-side project discussed here includes Server, WebAssembly, and Auto modes, not just the Server mode specifically.**

#### Install NuGet Package

Run the following command to install the required package:
```bash
dotnet add package DependencyInjection.StaticAccessor.Blazor
```

The initialization process for Blazor Server is similar to that of an AspNetCore project. Refer to the [AspNetCore Project Initialization Example](#aspnetcore项目初始化示例) for detailed instructions. However, since Blazor's DI scope differs from the standard AspNetCore DI scope, additional steps are required.

#### Pages Inherit from `PinnedScopeComponentBase`

Blazor's unique DI scope requires that all pages inherit from `PinnedScopeComponentBase`. It is recommended to define this inheritance globally in the `_Imports.razor` file so that it applies to all pages.

```csharp
// _Imports.razor

@inherits DependencyInjection.StaticAccessor.Blazor.PinnedScopeComponentBase
```

In addition to `PinnedScopeComponentBase`, the library provides `PinnedScopeOwningComponentBase` and `PinnedScopeLayoutComponentBase`, with the possibility of adding more types as needed in the future.

**Special Note**  
By default, when a page inherits from `PinnedScopeOwningComponentBase`, accessing `PinnedScope.ScopedServices` within callback methods or subsequent method calls retrieves the same object as `[Inject]`'s `IServiceProvider`. This behavior may differ from the expected `OwningComponentBase.ScopedServices`. To achieve the expected behavior, after calling `UsePinnedScopeServiceProvider` during initialization, call the `UseOwningScopedServices` extension method as shown below.

#### Example Initialization and Page Usage

```csharp
// Main
var builder = WebApplication.CreateBuilder(args);

// Add required services and configurations here...

builder.Host
    .UsePinnedScopeServiceProvider()  // Initializes PinnedScope
    .UseOwningScopedServices();       // Ensures PinnedScope.ScopedServices maps to OwningComponentBase.Services in inherited pages

var app = builder.Build();

// Configure the middleware pipeline...

app.Run();
```

#### Sample Razor Page: `Test.razor`

```csharp
@page "/test"
@using DependencyInjection.StaticAccessor
@using DependencyInjection.StaticAccessor.Blazor
@rendermode InteractiveServer
@inject IServiceProvider serviceProvider
@inherits PinnedScopeOwningComponentBase

<PageTitle>Test</PageTitle>

<button class="btn btn-primary" @onclick="Call">Click</button>

@code {
    private void Call()
    {
        var equals1 = serviceProvider == PinnedScope.ScopedServices;
        var equals2 = ScopedServices == PinnedScope.ScopedServices;

        /**
         * Results based on initialization:
         * 1. Without calling UseOwningScopedServices:
         *     equals1: true, equals2: false
         *
         * 2. With UseOwningScopedServices called during initialization:
         *     equals1: false, equals2: true
         */
    }
}
```

By following this setup, the behavior of `PinnedScope.ScopedServices` will align with your expectations, ensuring compatibility with Blazor's scoped DI features while offering flexibility for different inheritance scenarios.

#### Solution for Existing Custom ComponentBase Base Classes

If you are already using a custom `ComponentBase` base class provided by another package, C#'s lack of multiple inheritance can pose a challenge. Here are the recommended solutions:

1. If you can modify your base class, and it directly inherits `ComponentBase`, `OwningComponentBase`, or `LayoutComponentBase`

    Replace the base class with `PinnedScopeComponentBase`, `PinnedScopeOwningComponentBase`, or `PinnedScopeLayoutComponentBase`, depending on your use case.

2. If you can modify your base class, but it does not directly inherit from `ComponentBase`, `OwningComponentBase`, or `LayoutComponentBase`

    Modify your base class to implement the `IHandleEvent` and `IServiceProviderHolder` interfaces. Implement the interface methods based on the corresponding `PinnedScope` base class implementations.

    - [PinnedScopeComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeComponentBase.cs)
    - [PinnedScopeLayoutComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeLayoutComponentBase.cs)
    - [PinnedScopeOwningComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeOwningComponentBase.cs)

3. Unable to modify your base class

    Create a new custom base class that inherits from your existing base class, implements the `IHandleEvent` and `IServiceProviderHolder` interfaces and implements the required methods by following the corresponding `PinnedScope` base class implementations.

    - [PinnedScopeComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeComponentBase.cs)
    - [PinnedScopeLayoutComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeLayoutComponentBase.cs)
    - [PinnedScopeOwningComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeOwningComponentBase.cs)

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
