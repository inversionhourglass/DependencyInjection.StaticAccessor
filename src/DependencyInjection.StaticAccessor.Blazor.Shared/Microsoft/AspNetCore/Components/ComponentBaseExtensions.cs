using DependencyInjection.StaticAccessor;
using DependencyInjection.StaticAccessor.Blazor;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Components
{
    /// <summary>
    /// </summary>
    public static class ComponentBaseExtensions
    {
        private static readonly ConcurrentDictionary<Type, Func<EventCallbackWorkItem, object?, Task>> _Cache = [];

        /// <summary>
        /// Save the <see cref="IServiceProvider"/> before executing the callback.
        /// </summary>
        public static Task PinnedScopeHandleEventAsync<TComponent>(this TComponent component, EventCallbackWorkItem callback, object? arg) where TComponent : ComponentBase, IServiceProviderHolder
        {
            return component.PinnedScopeHandleEventAsync(component.GetType().BaseType!.BaseType!, callback, arg);
        }

        /// <summary>
        /// Save the <see cref="IServiceProvider"/> before executing the callback.
        /// </summary>
        public static Task PinnedScopeHandleEventAsync<TComponent>(this TComponent component, Type componentType, EventCallbackWorkItem callback, object? arg) where TComponent : ComponentBase, IServiceProviderHolder
        {
            var mHandleEventAsync = componentType.GetMethod("Microsoft.AspNetCore.Components.IHandleEvent.HandleEventAsync", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
            PinnedScope.Scope = new FoolScope(component.ServiceProvider);

            var func = _Cache.GetOrAdd(mHandleEventAsync.DeclaringType!, t => mHandleEventAsync.CreateDelegate<Func<EventCallbackWorkItem, object?, Task>>());

            return func(callback, arg);
        }
    }
}
