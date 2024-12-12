using Microsoft.AspNetCore.Components;

namespace DependencyInjection.StaticAccessor.Blazor
{
    /// <summary>
    /// Options for <see cref="PinnedScope"/>
    /// </summary>
    public class PinnedScopeOptions
    {
        /// <summary>
        /// If true, after inheriting <see cref="PinnedScopeOwningComponentBase"/>, the <see cref="PinnedScope.ScopedServices"/> is the same as <see cref="OwningComponentBase.ScopedServices"/>.
        /// </summary>
        public bool UseOwningScopedServices { get; set; }
    }
}
