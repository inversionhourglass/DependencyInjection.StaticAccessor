using DependencyInjection.StaticAccessor;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
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
            return hostBuilder.UseEditableServiceProvider((builder, context, options) =>
            {
                builder.Add(new ServiceScopeFactoryPinnedReplacer());
                configure?.Invoke(options);
            });
        }

        /// <summary>
        /// Use <see cref="EditableServiceProviderFactory"/> that can apply aspects before and after the <see cref="IServiceProviderFactory{TContainerBuilder}"/> builds.
        /// </summary>
        public static IHostBuilder UseEditableServiceProvider(this IHostBuilder hostBuilder, Action<ServiceProviderFactoryBuilder> configure)
        {
            return hostBuilder.UseEditableServiceProvider((builder, context, options) => configure(builder));
        }

        /// <summary>
        /// <inheritdoc cref="UseEditableServiceProvider(IHostBuilder, Action{ServiceProviderFactoryBuilder})"/>
        /// </summary>
        public static IHostBuilder UseEditableServiceProvider(this IHostBuilder hostBuilder, Action<ServiceProviderFactoryBuilder, ServiceProviderOptions> configure)
        {
            return hostBuilder.UseEditableServiceProvider((builder, context, options) => configure(builder, options));
        }

        /// <summary>
        /// <inheritdoc cref="UseEditableServiceProvider(IHostBuilder, Action{ServiceProviderFactoryBuilder})"/>
        /// </summary>
        public static IHostBuilder UseEditableServiceProvider(this IHostBuilder hostBuilder, Action<ServiceProviderFactoryBuilder, HostBuilderContext, ServiceProviderOptions> configure)
        {
            hostBuilder.UseServiceProviderFactory(context =>
            {
                var builder = ServiceProviderFactoryBuilder.CreateDefault();
                var options = new ServiceProviderOptions();

                configure(builder, context, options);

                return builder.Build(options);
            });

            return hostBuilder;
        }
    }
}
