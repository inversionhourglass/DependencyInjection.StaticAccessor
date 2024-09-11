using Microsoft.Extensions.DependencyInjection;
using System;

namespace DependencyInjection.StaticAccessor
{
    internal sealed class EditableServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly DefaultServiceProviderFactory _providerFactory;

        private readonly IBuilding[] _buildings;
        private readonly IBuilt[] _builts;

        public EditableServiceProviderFactory(ServiceProviderOptions? options, IBuilding[] buildings, IBuilt[] builts)
        {
            _providerFactory = options == null ? new() : new(options);
            _buildings = buildings;
            _builts = builts;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return _providerFactory.CreateBuilder(services);
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            foreach (var before in _buildings)
            {
                before.Handle(containerBuilder);
            }

            var provider = _providerFactory.CreateServiceProvider(containerBuilder);

            foreach (var after in _builts)
            {
                after.Handle(provider);
            }

            PinnedScope.ScopeProviders.AddRange(provider.GetServices<IScopeProvider>());
            PinnedScope.ScopeGuarders.AddRange(provider.GetServices<IScopeGuarder>());

            return provider;
        }
    }
}
