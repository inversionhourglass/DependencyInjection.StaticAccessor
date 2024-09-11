using DependencyInjection.StaticAccessor;
using System;

namespace Microsoft.AspNetCore.Components.WebAssembly.Hosting
{
    /// <summary>
    /// </summary>
    public static class StaticAccessorWebAssemblyExtensions
    {
        /// <summary>
        /// Set up <see cref="PinnedScope"/>.
        /// </summary>
        public static WebAssemblyHostBuilder UsePinnedScopeServiceProvider(this WebAssemblyHostBuilder hostBuilder)
        {
            return hostBuilder.UseEditableServiceProvider(builder =>
            {
                builder.Add(new ServiceScopeFactoryPinnedReplacer());
            });
        }

        /// <summary>
        /// <inheritdoc cref="UseEditableServiceProvider(WebAssemblyHostBuilder, Action{ServiceProviderFactoryBuilder})"/>
        /// </summary>
        public static WebAssemblyHostBuilder UseEditableServiceProvider(this WebAssemblyHostBuilder hostBuilder, Action<ServiceProviderFactoryBuilder> configure)
        {
            var builder = ServiceProviderFactoryBuilder.CreateDefault();

            configure(builder);

            var factory = builder.Build();

            hostBuilder.ConfigureContainer(factory);

            return hostBuilder;
        }
    }
}
