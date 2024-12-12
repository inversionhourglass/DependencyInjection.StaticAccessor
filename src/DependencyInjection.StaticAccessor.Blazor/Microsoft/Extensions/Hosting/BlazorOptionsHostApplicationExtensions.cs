using DependencyInjection.StaticAccessor.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// <see cref="HostApplicationBuilder"/> extension methods.
    /// </summary>
    public static class BlazorOptionsHostApplicationExtensions
    {
        /// <summary>
        /// Set <see cref="PinnedScopeOptions.UseOwningScopedServices"/> to true.
        /// </summary>
        public static HostApplicationBuilder UseOwningScopedServices(this HostApplicationBuilder builder)
        {
            builder.Services.AddOptions<PinnedScopeOptions>().Configure(options => options.UseOwningScopedServices = true);

            return builder;
        }
    }
}
