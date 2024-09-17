using DependencyInjection.StaticAccessor;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// </summary>
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// </summary>
        public static IServiceCollection AddScopeForward(this IServiceCollection services, Type forwardType)
        {
            return services.AddSingleton(typeof(IScopeForward), forwardType);
        }

        /// <summary>
        /// </summary>
        public static IServiceCollection AddScopeForward<T>(this IServiceCollection services) where T : class, IScopeForward
        {
            return services.AddSingleton<IScopeForward, T>();
        }

        /// <summary>
        /// </summary>
        public static IServiceCollection AddScopeGoalie(this IServiceCollection services, Type goalieType)
        {
            return services.AddSingleton(typeof(IScopeGoalie), goalieType);
        }

        /// <summary>
        /// </summary>
        public static IServiceCollection AddScopeGoalie<T>(this IServiceCollection services) where T : class, IScopeGoalie
        {
            return services.AddSingleton<IScopeGoalie, T>();
        }
    }
}
