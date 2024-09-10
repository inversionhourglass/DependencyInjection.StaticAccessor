using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// Save all the scopes that <see cref="PinnedServiceScopeFactory"/> created. Get <see cref="IServiceProvider"/> and <see cref="IServiceScope"/> anywhere.
    /// </summary>
    public sealed class PinnedScope : IServiceScope
    {
        private readonly IServiceScope _scope;

        private static readonly AsyncLocal<ScopeChain?> _Scope = new();

        /// <summary>
        /// </summary>
        internal PinnedScope(IServiceScope scope)
        {
            _scope = scope;
            Scope = scope;
        }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public IServiceProvider ServiceProvider => _scope.ServiceProvider;

        /// <summary>
        /// Get current scope. Return null if not within a scope.
        /// </summary>
        public static IServiceScope? Scope
        {
            get
            {
                return _Scope.Value?.Current;
            }
            internal set
            {
                var scope = _Scope.Value;

                if (value == null)
                {
                    if (scope == null) return;
                    _Scope.Value = scope.Parent;
                }
                else
                {
                    _Scope.Value = new(value, scope);
                }
            }
        }

        /// <summary>
        /// Get the <see cref="IServiceProvider"/> of the current scope, or the root <see cref="IServiceProvider"/> if not within a scope, or null if you have not set up the <see cref="PinnedServiceScopeFactory"/>.
        /// </summary>
        public static IServiceProvider? Services => Scope?.ServiceProvider ?? RootServices;

        /// <summary>
        /// Get the root <see cref="IServiceProvider"/>
        /// </summary>
        public static IServiceProvider? RootServices { get; internal set; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public void Dispose()
        {
            Scope = null;
            _scope.Dispose();
        }

        /// <summary>
        /// </summary>
        internal sealed class ScopeChain(IServiceScope scope, ScopeChain? parent)
        {
            public IServiceScope Current { get; } = scope;

            public ScopeChain? Parent { get; } = parent;
        }
    }
}
