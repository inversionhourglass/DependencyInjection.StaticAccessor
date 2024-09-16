using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// </summary>
    public abstract class FoolScopeProvider : IScopeProvider
    {
        private readonly Lazy<IServiceScope> _scope;

        /// <summary>
        /// </summary>
        public FoolScopeProvider()
        {
            _scope = new(New);
        }

        /// <summary>
        /// The <see cref="IServiceProvider"/> within the current scope.
        /// </summary>
        public abstract IServiceProvider ScopedService { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IServiceScope? Scope => _scope.Value;

        private IServiceScope New() => new FoolScope(ScopedService);
    }
}
