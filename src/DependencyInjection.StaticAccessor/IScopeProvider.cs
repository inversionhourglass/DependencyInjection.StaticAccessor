using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// <see cref="IServiceScope"/> provider
    /// </summary>
    public interface IScopeProvider
    {
        /// <summary>
        /// The current <see cref="IServiceScope"/>
        /// </summary>
        IServiceScope? Scope { get; }
    }
}
