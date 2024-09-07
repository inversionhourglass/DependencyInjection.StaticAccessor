using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace DependencyInjection.StaticAccessor.Hosting
{
    /// <summary>
    /// </summary>
    public static class StaticAccessorHostingExtensions
    {
        /// <summary>
        /// Set up <see cref="PinnedScope"/>.
        /// </summary>
        public static IHostBuilder UsePinnedScopeServiceProvider(this IHostBuilder hostBuilder, Action<ServiceProviderOptions>? configure = null)
        {
            return hostBuilder.UseEditableServiceProvider(ServiceProviderFactoryBuilder.CreatePinned(), configure);
        }

        /// <summary>
        /// Use <see cref="EditableServiceProviderFactory"/> that can apply aspects before and after the <see cref="IServiceProviderFactory{TContainerBuilder}"/> builds.
        /// </summary>
        public static IHostBuilder UseEditableServiceProvider(this IHostBuilder hostBuilder, ServiceProviderFactoryBuilder factoryBuilder, Action<ServiceProviderOptions>? configure = null)
        {
            ServiceProviderOptions? options = null;
            if (configure != null)
            {
                options = new ServiceProviderOptions();
                configure(options);
            }

            var factory = factoryBuilder.Build(options);
            hostBuilder.UseServiceProviderFactory(factory);

            return hostBuilder;
        }
    }
}
