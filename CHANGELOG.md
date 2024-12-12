# 9.0

- 支持.NET 9.0，同时将微软官方系列NuGet更新到9.0版本。
- 由于无法从`HostApplicationBuilder`对象获取`IHostBuilder`对象，所以对为`HostApplicationBuilder`新增扩展方法`UsePinnedScopeServiceProvider`和`UseEditableServiceProvider`。