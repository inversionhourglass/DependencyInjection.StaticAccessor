# DependencyInjection.StaticAccessor

中文 | [English](README_en.md)

`DependencyInjection.StaticAccessor`致力于为.NET各种类型的项目提供静态方式访问当前DI Scope对应的`IServiceProvider`对象，你可以很方便的在静态方法以及无法直接与DI服务交互的类型的方法中使用`IServiceProvider`。

## NuGet包一览

|                          包名                         |                               用途                              |
|:-----------------------------------------------------:|:---------------------------------------------------------------|
| DependencyInjection.StaticAccessor.Hosting            | 用于AspNetCore项目（WebApi、Mvc等）及通用主机（Generic Host）     |
| DependencyInjection.StaticAccessor.Blazor             | 用于Blazor项目，Blazor Server和Blazor WebAssembly Server都用这个 |
| DependencyInjection.StaticAccessor.Blazor.WebAssembly | 用于Blazor WebAssembly Client项目，同样支持Auto模式Client项目     |
| DependencyInjection.StaticAccessor                    | 基础类库，一般不直接引用                                         |

### 版本号说明

所有版本号格式都采用语义版本号（SemVer），主版本号与`Microsoft.Extensions.*`保持一致（**引用NuGet时请保持主版本号与你引用的`Microsoft.Extensions.*`保持一致**），次版本号作为功能更新版本号，修订号为bug修复及微小改动版本号。

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

**注意，这里的Blazor Server端是包含Server, WebAssembly, Auto三种模式的Server端项目，不是单指Server模式。**

安装NuGet引用
> dotnet add package DependencyInjection.StaticAccessor.Blazor

Blazor Server端初始化操作与AspNetCore相同，直接参考[AspNetCore项目初始化示例](#aspnetcore项目初始化示例)，这里不再赘述。不过由于Blazor的DI scope与常规的不同，所以还需要做一些额外的操作。

#### 页面继承PinnedScopeComponentBase

Blazor的特殊DI scope需要所有页面需要继承自`PinnedScopeComponentBase`，推荐做法是在`_Imports.razor`直接定义，一次定义所有页面都生效。

```csharp
// _Imports.razor

@inherits DependencyInjection.StaticAccessor.Blazor.PinnedScopeComponentBase

```

除了`PinnedScopeComponentBase`，还提供了`PinnedScopeOwningComponentBase`和`PinnedScopeLayoutComponentBase`，后续会根据需要可能会加入更多类型。

#### 已有自定义ComponentBase基类的解决方案

你可能会使用其他包定义的`ComponentBase`基类，C#不支持多继承，所以这里提供了不继承`PinnedScopeComponentBase`的解决方案。

```csharp
// 假设你现在使用的ComponentBase基类是ThirdPartyComponentBase

// 定义新的基类继承ThirdPartyComponentBase
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

虽然你可以通过`PinnedScope.Scope`获取当前的DI Scope，但请不要通过该属性直接操作`PinnedScope.Scope`，比如调用`Dispose`方法，你应该通过你创建scope时创建的变量进行操作。

### 不支持非通常Scope

一般日常开发时不需要关注这个问题的，通常的AspNetCore项目也不会出现这样的场景，Blazor就是官方项目类型中非通常DI Scope的案例。

在解释什么是非通常Scope前，我先聊聊通常的Scope模式。我们知道DI Scope是可以嵌套的，在通常情况下，嵌套的Scope呈现的是一种栈的结构，后创建的scope先释放，井然有序。

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

了解了非通常Scope，那么就很好理解非通常Scope了，只要是不按照这种井然有序的栈结构的，那就是非通常Scope。比较常见的就是Blazor的这种情况：

我们知道，Blazor SSR通过SignalR实现SPA，一个SignalR连接对应一个DI Scope，界面上的各种事件（点击、获取焦点等）通过SignalR通知服务端回调事件函数，而这个回调便是从外部横插一脚与SignalR进行交互的，在不进行特殊处理的情况下，回调事件所属的Scope是当前回调事件新创建的Scope，但我们在回调事件中与之交互的`Component`是SignalR所属Scope创建的，这就出现了Scope交叉交互的情况。`PinnedScopeComponentBase`所做的便是在执行回调函数之前，将`PinnedScope.Scope`重设回SignalR对应Scope。
