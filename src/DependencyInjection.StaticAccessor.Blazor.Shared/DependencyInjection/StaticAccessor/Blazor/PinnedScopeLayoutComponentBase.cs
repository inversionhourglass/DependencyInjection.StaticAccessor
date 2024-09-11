using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace DependencyInjection.StaticAccessor.Blazor
{
    /// <summary>
    /// Inherit from <see cref="LayoutComponentBase"/> and automatically set <see cref="PinnedScope.Scope"/>
    /// </summary>
    public class PinnedScopeLayoutComponentBase : LayoutComponentBase, IHandleEvent, IServiceProviderHolder
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private IServiceProvider _serviceProvider;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        [Inject]
        public IServiceProvider ServiceProvider
        {
            get => _serviceProvider;
            set
            {
                PinnedScope.Scope = new FoolScope(value);
                _serviceProvider = value;
            }
        }

        Task IHandleEvent.HandleEventAsync(EventCallbackWorkItem callback, object? arg)
        {
            return this.PinnedScopeHandleEventAsync(callback, arg);
        }
    }
}
