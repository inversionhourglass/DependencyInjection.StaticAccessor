using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Preferentially obtain the <see cref="IServiceScope"/> from the <see cref="IServiceProvider"/>s,
    /// return the default <see cref="IServiceScope"/> if all <see cref="IScopeProvider.Scope"/> instances return null.
    /// </summary>
    public interface IScopeProvider
    {
        /// <summary>
        /// The current <see cref="IServiceScope"/>
        /// </summary>
        IServiceScope? Scope { get; }
    }
}
