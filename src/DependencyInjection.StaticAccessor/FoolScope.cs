using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Just keep the <see cref="IServiceProvider"/> that you gave
    /// </summary>
    /// <param name="provider"></param>
    public class FoolScope(IServiceProvider provider) : IServiceScope
    {
        /// <summary>
        /// The <see cref="IServiceProvider"/> that you gave
        /// </summary>
        public IServiceProvider ServiceProvider => provider;

        /// <summary>
        /// <see cref="FoolScope"/> do nothing with Dispose
        /// </summary>
        public void Dispose()
        {
            // do nothing
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override int GetHashCode()
        {
            return provider.GetHashCode();
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is IServiceScope scope && scope.ServiceProvider == provider;
        }
    }
}
