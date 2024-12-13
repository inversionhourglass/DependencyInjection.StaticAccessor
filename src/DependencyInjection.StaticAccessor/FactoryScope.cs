using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Keep a factory of <see cref="IServiceProvider"/>. It is for delayed creation.
    /// </summary>
    public readonly struct FactoryScope(Func<IServiceProvider> factory) : IServiceScope
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> that factory created
        /// </summary>
        public IServiceProvider ServiceProvider => factory();

        /// <summary>
        /// <see cref="FactoryScope"/> do nothing with Dispose
        /// </summary>
        public void Dispose()
        {
            // do nothing
        }
    }
}
