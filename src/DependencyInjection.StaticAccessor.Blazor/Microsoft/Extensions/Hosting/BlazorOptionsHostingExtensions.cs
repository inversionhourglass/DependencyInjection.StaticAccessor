using DependencyInjection.StaticAccessor.Blazor;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// <see cref="IHostBuilder"/> extension methods
    /// </summary>
    public static class BlazorOptionsHostingExtensions
    {
        /// <summary>
        /// Set <see cref="PinnedScopeOptions.UseOwningScopedServices"/> to true.
        /// </summary>
        public static IHostBuilder UseOwningScopedServices(this IHostBuilder builder)
        {
            return builder.ConfigureServices(services =>
            {
                services.AddOptions<PinnedScopeOptions>().Configure(options => options.UseOwningScopedServices = true);
            });
        }
    }
}
