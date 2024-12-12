using DependencyInjection.StaticAccessor;
using DependencyInjection.StaticAccessor.Blazor;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// </summary>
    public static class ComponentBaseExtensions
    {
        /// <summary>
        /// Save the <see cref="IServiceProvider"/> before executing the callback.
        /// </summary>
        public static Task PinnedScopeHandleEventAsync<TComponent>(this TComponent component, EventCallbackWorkItem callback, object? arg) where TComponent : ComponentBase, IScopeCreator
        {
            PinnedScope.Scope = component.Create();

            return HandleEventAsync(component, callback, arg);
        }

        [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "Microsoft.AspNetCore.Components.IHandleEvent.HandleEventAsync")]
        private extern static Task HandleEventAsync(ComponentBase handleEvent, EventCallbackWorkItem callback, object? arg);
    }
}
