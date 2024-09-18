using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Preferentially obtain the <see cref="IServiceScope"/> from the <see cref="IScopeForward"/>s,
    /// return the default <see cref="IServiceScope"/> if all the instances return null.
    /// </summary>
    public interface IScopeForward : IScopeProvider
    {
    }
}
