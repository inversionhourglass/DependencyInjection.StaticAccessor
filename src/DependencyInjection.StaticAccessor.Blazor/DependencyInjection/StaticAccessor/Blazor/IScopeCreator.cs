using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor.Blazor
{
    /// <summary>
    /// Current scoped service holder
    /// </summary>
    public interface IScopeCreator
    {
        /// <summary>
        /// Create a <see cref="IServiceScope"/>
        /// </summary>
        IServiceScope Create();
    }
}
