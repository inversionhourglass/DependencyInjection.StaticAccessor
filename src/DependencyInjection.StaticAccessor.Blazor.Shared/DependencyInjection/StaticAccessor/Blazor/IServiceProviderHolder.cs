using System;

namespace DependencyInjection.StaticAccessor.Blazor
{
    /// <summary>
    /// Current scoped service holder
    /// </summary>
    public interface IServiceProviderHolder
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> within the current scope.
        /// </summary>
        IServiceProvider ServiceProvider { get; set; }
    }
}
