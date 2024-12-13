# 8.1

1. .NET 7.0 新增`HostApplicationBuilder`，但由于无法从`HostApplicationBuilder`对象获取`IHostBuilder`对象，所以对为`HostApplicationBuilder`新增扩展方法`UsePinnedScopeServiceProvider`和`UseEditableServiceProvider`。
2. 使用`UnsafeAccessorAttribute`替代反射访问`ComponentBase.HandleEventAsync`，提升访问性能
3. 针对 Blazor 项目，为`IHostBuilder`和`HostApplicationBuilder`新增`UseOwningScopedServices`扩展方法。在应用初始化时调用该方法后，对于继承了`PinnedScopeOwningComponentBase`的页面的事件回调方法及后续调用方法，通过`PinnedScope.ScopedServices`获取的`IServiceProvider`等同于`OwningComponentBase.ScopedServices`。

    ```csharp
    // Main
    var builder = WebApplication.CreateBuilder(args);

    // ...

    builder.Host
        .UsePinnedScopeServiceProvider()  // 调用该方法完成 PinnedScope 初始化
        .UseOwningScopedServices();       // 调用该方法后，对于继承 PinnedScopeOwningComponentBase 的页面，在回调方法及后续调用方法中，通过 PinnedScope.ScopedServices 获取到的为 OwningCompoenetBase.Service

    var app = builder.Build();

    // ...

    app.Run();


    // Test.razor
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
             * 1. 如果初始化时不调用 UseOwningScopedServices 扩展方法，结果如下
             *     equals1: true, equals2: false
             *
             * 2. 如果初始化时调用 UseOwningScopedServices 扩展方法，结果如下
             *     equals1: false, equals2: true
             */
        }
    }
    ```

4. 为了实现第四项的功能，修改了`PinnedScopeComponentBase`, `PinnedScopeOwningComponentBase`和`PinnedScopeLayoutComponentBase`的实现，如果之前按照 [已有自定义ComponentBase基类的解决方案](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/master/README.md#%E5%B7%B2%E6%9C%89%E8%87%AA%E5%AE%9A%E4%B9%89componentbase%E5%9F%BA%E7%B1%BB%E7%9A%84%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88) 修改了`ComponentBase`系列基类，现在你需要重新参照最新的 [已有自定义ComponentBase基类的解决方案](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor/blob/legacy/8.0/README.md#%E5%B7%B2%E6%9C%89%E8%87%AA%E5%AE%9A%E4%B9%89componentbase%E5%9F%BA%E7%B1%BB%E7%9A%84%E8%A7%A3%E5%86%B3%E6%96%B9%E6%A1%88) 进行修改。