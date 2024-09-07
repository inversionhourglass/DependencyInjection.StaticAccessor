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
                return _Scope.Value?.Peek();
            }
            internal set
            {
                var scope = _Scope.Value;

                if (value == null)
                {
                    if (scope == null) return;
                    if (!scope.TryPop()) _Scope.Value = null;
                }
                else
                {
                    if (scope == null)
                    {
                        _Scope.Value = scope = new();
                    }
                    scope.Push(value);
                }
            }
        }

        /// <summary>
        /// Get the <see cref="IServiceProvider"/> of the current scope, or the root <see cref="IServiceProvider"/> if not within a scope, or null if you have not set up the <see cref="PinnedServiceScopeFactory"/>.
        /// </summary>
        public static IServiceProvider? Services => Scope?.ServiceProvider;

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
        internal sealed class ScopeChain : Stack<IServiceScope>
        {
            /// <summary>
            /// </summary>
            public bool TryPop()
            {
#if NETSTANDARD2_0
                if (Count > 0)
                {
                    Pop();
                    return true;
                }
                return false;
#else
                return TryPop(out _);
#endif
            }
        }
    }
}
