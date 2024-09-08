using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DependencyInjection.StaticAccessor
{
    /// <summary>
    /// <see cref="EditableServiceProviderFactory"/> builder.
    /// </summary>
    public sealed class ServiceProviderFactoryBuilder
    {
        private readonly List<IBuilding> _buildings = [];
        private readonly List<IBuilt> _builts = [];

        /// <summary>
        /// Add a <see cref="IBuilding"/> to <see cref="EditableServiceProviderFactory"/>
        /// </summary>
        public ServiceProviderFactoryBuilder Add(IBuilding building)
        {
            _buildings.Add(building);
            return this;
        }

        /// <summary>
        /// Add a <see cref="IBuilt"/> to <see cref="EditableServiceProviderFactory"/>
        /// </summary>
        public ServiceProviderFactoryBuilder Add(IBuilt built)
        {
            _builts.Add(built);
            return this;
        }

        /// <summary>
        /// Build a <see cref="EditableServiceProviderFactory"/>
        /// </summary>
        public IServiceProviderFactory<IServiceCollection> Build(ServiceProviderOptions? options = null)
        {
            return new EditableServiceProviderFactory(options, _buildings.ToArray(), _builts.ToArray());
        }

        /// <summary>
        /// Create an empty <see cref="ServiceProviderFactoryBuilder"/>
        /// </summary>
        public static ServiceProviderFactoryBuilder CreateDefault() => new();
    }
}
