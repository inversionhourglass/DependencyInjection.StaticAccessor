# DependencyInjection.StaticAccessor

中文 | [English](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/README_en.md)

`DependencyInjection.StaticAccessor`致力于为.NET各种类型的项目提供静态方式访问当前 DI Scope 对应的`IServiceProvider`对象，你可以很方便的在静态方法以及无法直接与 DI 服务交互的类型的方法中使用`IServiceProvider`。

## 为什么需要PinnedScope

我们在使用依赖注入的过程中，经常碰到的一个问题便是，如何在静态方法中访问 IoC/DI 容器。对于这个问题，有很多的解决方案，最简单方式便是在应用启动时获取根容器保存到静态变量中，之后直接通过该静态变量访问即可。

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

这种方式对于简单场景非常适用，操作简单且有效。但我们知道，Microsoft Dependency Injection 服务生命周期有三种：`Transient`, `Scoped`, `Singleton`。其中比较特殊的便是`Scoped`，在 Web 开发中，最常接触到的也是`Scoped`，一个请求便对应着一个`Scope`。对于注册为`Scoped`的对象，我们无法直接从根容器获取，而需要`Scope`容器中获取。对于这种需求，在 Web 程序中依旧有很简单的解决方案。Microsoft 提供了`IHttpContextAccessor`，我们可以从根容器获取`IHttpContextAccessor`对象，然后通过`IHttpContextAccessor.HttpContext.RequestServices`获取当前请求对应的`IServiceProvider`。

既然都有简单的解决方案，那为什么需要`PinnedScope`呢？因为在复杂场景下，Scope 的使用并不是这么简单。比如在 Web 应用中，我们可能需要开启一个后台线程完成一些异步操作，同时请求不必等待这个异步操作完成。在异步操作中，我们也会使用 DI 容器，那么此时，我们在一个静态方法中应该如何获取 DI 容器呢？继续通过根容器和`IHttpContextAccessor`访问是否可行？答案是不行的，因为此时请求可能已经处理完毕，通过`IHttpContextAccessor`将访问到一个已释放的`IServiceProvider`。对于这种场景，我们一般会为异步操作单独创建一个 Scope，此时你可以选择自己管理这些 Scope，但你也可以选择直接使用`PinnedScope`。

上面只是描述了一个简单的场景，其实在很多情况下 Scope 的使用并不是那么简单，比如任务调度、消息订阅以及官方的 Blazor 等。当然，你可以为每一种场景找到适合的方式访问`IServiceProvider`，`PinnedScope`只是为你提供了一个简单而统一的方式。你不必关心你所使用的框架如何创建和管理 Scope，你只需要通过`PinndeScope`即可获取到正确的`IServiceProvider`。

## NuGet包一览

|                          包名                         |                                  用途                                |
|:-----------------------------------------------------:|:--------------------------------------------------------------------|
| DependencyInjection.StaticAccessor.Hosting            | 用于 AspNetCore 项目（WebApi、Mvc 等）及通用主机（Generic Host）       |
| DependencyInjection.StaticAccessor.Blazor             | 用于 Blazor 项目，Blazor Server 和 Blazor WebAssembly Server 都用这个 |
| DependencyInjection.StaticAccessor.Blazor.WebAssembly | 用于 Blazor WebAssembly Client 项目，同样支持 Auto 模式 Client 项目    |
| DependencyInjection.StaticAccessor                    | 基础类库，使用`PinnedScope`的非启动项目引用该类库                       |

### 版本号说明

所有版本号格式都采用语义版本号（SemVer），主版本号与`Microsoft.Extensions.*`保持一致（**引用NuGet时请保持主版本号与你引用的`Microsoft.Extensions.*`保持一致**），次版本号作为功能更新版本号，修订号为 BUG 修复及微小改动版本号。

## 快速开始

根据项目类型，参考[nuget包一览](#nuget包一览)安装对应NuGet。

```csharp
// 1. 初始化（通用主机）
var builder = Host.CreateDefaultBuilder();

builder.UsePinnedScopeServiceProvider(); // 仅此一步完成初始化

var host = builder.Build();

host.Run();

// 2. 在任何地方获取
class Test
{
    public static void M()
    {
        var yourService = PinnedScope.ScopedServices.GetService<IYourService>();
    }
}
```

不同类型的项目初始化方式类似，都是调用扩展方法`UsePinnedScopeServiceProvider`，后面会给出不同类型项目初始化的示例代码。

### AspNetCore项目初始化示例

安装NuGet引用
> dotnet add package DependencyInjection.StaticAccessor.Hosting

```csharp
// 1. 初始化
var builder = WebApplication.CreateBuilder();

builder.Host.UsePinnedScopeServiceProvider(); // 仅此一步完成初始化

var app = builder.Build();

app.Run();
```

### Blazor Server端项目初始化

**注意，这里的 Blazor Server 端是包含 Server, WebAssembly, Auto 三种模式的 Server 端项目，不是单指 Server 模式。**

安装NuGet引用
> dotnet add package DependencyInjection.StaticAccessor.Blazor

Blazor Server 端初始化操作与 AspNetCore 相同，直接参考[AspNetCore项目初始化示例](#aspnetcore项目初始化示例)，这里不再赘述。不过由于 Blazor 的 DI Scope 与常规的不同，所以还需要做一些额外的操作。

#### 页面继承PinnedScopeComponentBase

Blazor 的特殊 DI Scope 需要所有页面需要继承自`PinnedScopeComponentBase`，推荐做法是在`_Imports.razor`直接定义，一次定义所有页面都生效。

```csharp
// _Imports.razor

@inherits DependencyInjection.StaticAccessor.Blazor.PinnedScopeComponentBase
```

除了`PinnedScopeComponentBase`，还提供了`PinnedScopeOwningComponentBase`和`PinnedScopeLayoutComponentBase`，后续会根据需要可能会加入更多类型。

**需要特别说明的是，在 8.1 及之后的版本中，默认情况下，继承`PinnedScopeOwningComponentBase`后，在该页面的回调方法及后续调用方法中访问`PinnedScope.ScopedServices`，获取到的对象为`OwningComponentBase.ScopedServies`。这一行为更符合预期，8.0 版本获取到的是与 inject 注入的`IServiceProvider`相同。**

#### 已有自定义ComponentBase基类的解决方案

你可能会使用其他包定义的`ComponentBase`基类，C#不支持多继承，所以这里提供对应的解决方案：

1. 你可以修改你的基类，且你的基类直接继承`ComponentBase`或`OwningCompoenetBase`或`LayoutComponentBase`

    直接将基类的父类修改为对应的`PinnedScope`基类即可（`PinnedScopeComponentBase / PinnedScopeOwningComponentBase / PinnedScopeLayoutComponentBase`）

2. 你可以修改你的基类，但是你的基类不是直接继承自`ComponentBase`或`OwningCompoenetBase`或`LayoutComponentBase`

    修改基类修改实现`IHandleEvent`和`IServiceProviderHolder`接口，参照`PinnedScope`对应基类的实现代码实现接口方法。
    - [PinnedScopeComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeComponentBase.cs)
    - [PinnedScopeLayoutComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeLayoutComponentBase.cs)
    - [PinnedScopeOwningComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeOwningComponentBase.cs)

3. 你无法修改你的基类

    自定义基类实现当前基类，同时实现`IHandleEvent`和`IServiceProviderHolder`接口，并参照`PinnedScope`对应基类的实现代码实现接口方法。
    - [PinnedScopeComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeComponentBase.cs)
    - [PinnedScopeLayoutComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeLayoutComponentBase.cs)
    - [PinnedScopeOwningComponentBase](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/d08ac948e562df4bf2ec5fcbddb6858f12a18636/src/DependencyInjection.StaticAccessor.Blazor/DependencyInjection/StaticAccessor/Blazor/PinnedScopeOwningComponentBase.cs)

### Blazor WebAssembly Client初始化

**注意，这里是Blazor WebAssembly Client端的初始化，Server端的初始化请查看[Blazor Server端项目初始化](#blazor-server端项目初始化)**

```csharp
var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.UsePinnedScopeServiceProvider(); // 仅此一步完成初始化

await builder.Build().RunAsync();
```

与Server端相同，Client端的页面也需要继承`PinnedScopeComponentBase`，请参考[页面继承PinnedScopeComponentBase](#页面继承pinnedscopecomponentbase)。

## 注意事项

### 不要通过PinnedScope操作IServiceScope

虽然你可以通过`PinnedScope.Scope`获取当前的 DI Scope，但请不要通过该属性直接操作`PinnedScope.Scope`，比如调用`Dispose`方法，你应该通过你创建 Scope 时创建的变量进行操作。

### 不支持非通常Scope

一般日常开发时不需要关注这个问题的，通常的 AspNetCore 项目也不会出现这样的场景，Blazor 就是官方项目类型中非通常 DI Scope 的案例。

在解释什么是非通常 Scope 前，我先聊聊通常的 Scope 模式。我们知道 DI Scope 是可以嵌套的，在通常情况下，嵌套的 Scope 呈现的是一种栈的结构，后创建的 Scope 先释放，井然有序。

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

    }                                                                  // pop scope22.  [scope22]
}                                                                      // pop scope11.  []
```

了解了非通常 Scope，那么就很好理解非通常 Scope 了，只要是不按照这种井然有序的栈结构的，那就是非通常 Scope。比较常见的就是 Blazor 的这种情况：

我们知道，Blazor SSR 通过 SignalR 实现 SPA，一个 SignalR 连接对应一个 DI Scope，界面上的各种事件（点击、获取焦点等）通过 SignalR 通知服务端回调事件函数，而这个回调便是从外部横插一脚与 SignalR 进行交互的，在不进行特殊处理的情况下，回调事件所属的 Scope 是当前回调事件新创建的 Scope，但我们在回调事件中与之交互的`Component`是 SignalR 所属 Scope 创建的，这就出现了 Scope 交叉交互的情况。`PinnedScopeComponentBase`所做的便是在执行回调函数之前，将`PinnedScope.Scope`重设回 SignalR 对应 Scope。
