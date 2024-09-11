using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// If the default scope value of <see cref="PinnedScope.Scope"/> is null, then try to get the scope from <see cref="IScopeGuarder"/> instances.
    /// </summary>
    public interface IScopeGuarder
    {
        /// <summary>
        /// The current <see cref="IServiceScope"/>
        /// </summary>
        IServiceScope? Scope { get; }
    }
}
