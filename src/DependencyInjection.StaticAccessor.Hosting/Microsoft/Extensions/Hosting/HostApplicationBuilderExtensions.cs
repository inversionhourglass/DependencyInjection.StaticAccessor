#if NET7_0_OR_GREATER
using DependencyInjection.StaticAccessor;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// <see cref="HostApplicationBuilder"/> extension methos
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// Set up <see cref="PinnedScope"/>.
        /// </summary>
        public static HostApplicationBuilder UsePinnedScopeServiceProvider(this HostApplicationBuilder hostAppBuilder, Action<ServiceProviderOptions>? configure = null)
        {
            return hostAppBuilder.UseEditableServiceProvider((builder, options) =>
            {
                builder.Add(new ServiceScopeFactoryPinnedReplacer());
                configure?.Invoke(options);
            });
        }

        /// <summary>
        /// Use <see cref="EditableServiceProviderFactory"/> that can apply aspects before and after the <see cref="IServiceProviderFactory{TContainerBuilder}"/> builds.
        /// </summary>
        public static HostApplicationBuilder UseEditableServiceProvider(this HostApplicationBuilder hostAppBuilder, Action<ServiceProviderFactoryBuilder> configure)
        {
            return hostAppBuilder.UseEditableServiceProvider((builder, options) => configure(builder));
        }

        /// <summary>
        /// <inheritdoc cref="UseEditableServiceProvider(HostApplicationBuilder, Action{ServiceProviderFactoryBuilder})"/>
        /// </summary>
        public static HostApplicationBuilder UseEditableServiceProvider(this HostApplicationBuilder hostAppBuilder, Action<ServiceProviderFactoryBuilder, ServiceProviderOptions> configure)
        {
            var builder = ServiceProviderFactoryBuilder.CreateDefault();
            var options = new ServiceProviderOptions();

            configure(builder, options);

            var factory = builder.Build(options);

            hostAppBuilder.ConfigureContainer(factory);

            return hostAppBuilder;
        }
    }
}
#endif
